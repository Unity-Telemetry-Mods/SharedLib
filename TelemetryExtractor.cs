using BepInEx.Logging;

using System;

using UnityEngine;

namespace TelemetryLib
{
    /// <summary>
    /// Specifies the method used to calculate Euler angles (pitch, yaw, roll).
    /// </summary>
    internal enum EulerType
    {
        /// <summary>
        /// Uses Unity's built-in Euler angle calculation.
        /// </summary>
        Unity,
        /// <summary>
        /// Uses a custom coaster-based calculation that computes yaw from angular velocity delta
        /// and pitch/roll from vector projections.
        /// </summary>
        Coaster
    }

    /// <summary>
    /// Extracts telemetry data from a Unity <see cref="Rigidbody"/>, including local velocities,
    /// angular velocities, acceleration, and orientation information.
    /// </summary>
    internal class TelemetryExtractor
    {
        private Rigidbody _rigidBody;

        /// <summary>
        /// Gets the velocity in the local space of the rigidbody.
        /// </summary>
        public Vector3 LocalVelocity { get; private set; } = Vector3.zero;

        /// <summary>
        /// Gets the angular velocity in the local space of the rigidbody.
        /// </summary>
        public Vector3 LocalAngularVelocity { get; private set; } = Vector3.zero;

        /// <summary>
        /// The centripetal force acting on the rigidbody, updated each call to <see cref="ExtractTelemetry"/>.
        /// </summary>
        public float CentripetalForce;

        

        /// <summary>
        /// Gets or sets the rigidbody being monitored.
        /// Logs a warning if accessed when null, and resets telemetry state when the value changes.
        /// </summary>
        protected Rigidbody rigidBody
        {
            get
            {
                if (_rigidBody == null)
                {
                    logger.LogWarning("Rigidbody is null, please call Update() method to set the rigidbody");
                }
                return _rigidBody;
            }
            private set
            {
                if(_rigidBody != value)
                {
                    logger.LogInfo($"Rigidbody changed! [{_rigidBody?.GetHashCode():x4}][{value.GetHashCode():x4}]");
                    _rigidBody = value;
                    this.Reset();
                }
                
            }
        }

        float _yaw = 0f;
        private Vector3 _previousLocalVelocity = Vector3.zero;

       

        ManualLogSource logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryExtractor"/> class
        /// and creates a BepInEx log source.
        /// </summary>
        public TelemetryExtractor()
        {
            //rigidBody = rigidBody;
            logger = BepInEx.Logging.Logger.CreateLogSource("TelemetryExtractor");
            logger.LogMessage("Creating new TelemetryExtractor");

        }

        /// <summary>
        /// Updates the rigidbody used for telemetry extraction.
        /// Must be called each frame before calling <see cref="ExtractTelemetry"/>.
        /// </summary>
        /// <param name="rigidBody">The <see cref="Rigidbody"/> to extract telemetry from.</param>
        public void Update(Rigidbody rigidBody)
        {
            this.rigidBody = rigidBody;
        }

        /// <summary>
        /// Extracts telemetry data from the current rigidbody, including rotation, local velocities,
        /// acceleration in G-forces, forward speed, and centripetal force.
        /// Returns an empty <see cref="BasicTelemetry"/> and resets state if the rigidbody is null.
        /// </summary>
        /// <param name="eulerType">The method used to calculate pitch, yaw, and roll. Defaults to <see cref="EulerType.Unity"/>.</param>
        /// <returns>A <see cref="BasicTelemetry"/> struct populated with the current frame's telemetry data.</returns>
        public BasicTelemetry ExtractTelemetry( EulerType eulerType = EulerType.Unity)
        {
            if(rigidBody == null)
            {
                Reset();
                return new BasicTelemetry();
            }

            var deltaTime = Time.fixedDeltaTime;

            var data = new BasicTelemetry();

           

            data.Rotation = rigidBody.transform.rotation;
            LocalAngularVelocity = rigidBody.transform.InverseTransformDirection(rigidBody.angularVelocity);
            LocalVelocity = rigidBody.transform.InverseTransformDirection(rigidBody.velocity);

            data.LocalAngularVelocity = LocalAngularVelocity;
            data.LocalVelocity = LocalVelocity;

            switch (eulerType)
            {
                case EulerType.Unity:
                    data.EulerAngles = GetPitchYawRoll( data.Rotation.eulerAngles);
                    break;
                case EulerType.Coaster:
                    _yaw += data.LocalAngularVelocity.y * deltaTime;
                    data.EulerAngles = GetCoasterEulerAngles();
                    break;
            }


            data.Accel = (data.LocalVelocity - _previousLocalVelocity) / deltaTime / Maths.GRAVITY;            

            

            data.Speed = ForwardVelocity(); 
            CentripetalForce = Maths.CalculateCentripetalAcceleration(LocalVelocity, LocalAngularVelocity);            

            data.CentripetalForce = CentripetalForce;

            return data;
        }

        /// <summary>
        /// Calculates yaw based on delta local velocity, pitch and roll based on projection of forward and right vectors
        /// </summary>
        /// <returns>Local Euler Angles</returns>
        private Vector3 GetCoasterEulerAngles()
        {
            

            var yaw = Maths.HemiCircle(_yaw * Mathf.Rad2Deg % 360);

            var pitch = Maths.CopySign(Vector3.Angle(new Vector3(rigidBody.transform.forward.x, 0, rigidBody.transform.forward.z), rigidBody.transform.forward), rigidBody.transform.forward.y);

            var roll = Maths.CopySign(Vector3.Angle(new Vector3(rigidBody.transform.right.x, 0, rigidBody.transform.right.z), rigidBody.transform.right), rigidBody.transform.right.y);

            return new Vector3(pitch, yaw, roll);
        }

        /// <summary>
        /// Converts Unity Euler angles to a pitch, yaw, roll vector.
        /// Pitch and roll are normalized to the [-180, 180] range via <c>Maths.HemiCircle</c>.
        /// </summary>
        /// <param name="eulerAngles">The Unity Euler angles to convert.</param>
        /// <returns>A <see cref="Vector3"/> containing pitch (x), yaw (y), and roll (z) in degrees.</returns>
        public Vector3 GetPitchYawRoll(Vector3 eulerAngles)
        {
            var pyr = new Vector3(
                    Maths.HemiCircle(eulerAngles.x),
                    eulerAngles.y, //Maths.HemiCircle(eulerAngles.y),
                    Maths.HemiCircle(eulerAngles.z));

            return pyr;
        }

        /// <summary>
        /// Calculates the forward speed of the rigidbody by projecting its world velocity
        /// onto its forward direction.
        /// </summary>
        /// <returns>The absolute forward velocity in units per second.</returns>
        private float ForwardVelocity()
        {
            return Mathf.Abs(Vector3.Dot(rigidBody.velocity, rigidBody.transform.forward));
        }

        /// <summary>
        /// Resets accumulated telemetry state, clearing the integrated yaw value.
        /// Called automatically when the rigidbody is null or changes.
        /// </summary>
        public void Reset()
        {
            logger.LogWarning("Rigidbody is null, resetting yaw");
            _yaw = 0;
        }

    }

    /// <summary>
    /// Contains basic telemetry data extracted from a <see cref="Rigidbody"/>, including
    /// orientation, local velocities, acceleration, forward speed, and centripetal force.
    /// </summary>
    internal struct BasicTelemetry
    {
        /// <summary>
        /// The rotation of the rigidbody as a quaternion.
        /// </summary>
        public Quaternion Rotation;

        /// <summary>
        /// The orientation as Euler angles (pitch, yaw, roll) in degrees.
        /// </summary>
        public Vector3 EulerAngles;

        /// <summary>
        /// The angular velocity in the local space of the rigidbody, in radians per second.
        /// </summary>
        public Vector3 LocalAngularVelocity;

        /// <summary>
        /// The velocity in the local space of the rigidbody, in units per second.
        /// </summary>
        public Vector3 LocalVelocity;

        /// <summary>
        /// The acceleration expressed in G-forces (1G = <c>Maths.GRAVITY</c> m/s²).
        /// </summary>
        public Vector3 Accel;

        /// <summary>
        /// The forward speed of the rigidbody in units per second.
        /// </summary>
        public float Speed;

        /// <summary>
        /// The centripetal acceleration acting on the rigidbody.
        /// </summary>
        public float CentripetalForce;

    }
}

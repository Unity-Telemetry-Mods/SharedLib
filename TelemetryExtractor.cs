using BepInEx.Logging;

using System;

using UnityEngine;

namespace TelemetryLib
{
    internal enum EulerType
    {
        Unity,
        Coaster
    }

    internal class TelemetryExtractor
    {
        private Rigidbody _rigidBody;

        public Vector3 LocalVelocity { get; private set; } = Vector3.zero;
        public Vector3 LocalAngularVelocity { get; private set; } = Vector3.zero;

        public float CentripetalForce;

        

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
        public TelemetryExtractor()
        {
            //rigidBody = rigidBody;
            logger = BepInEx.Logging.Logger.CreateLogSource("TelemetryExtractor");
            logger.LogMessage("Creating new TelemetryExtractor");

        }

        /// <summary>
        /// You must call this method to update the rigidbody
        /// </summary>
        /// <param name="rigidBody"></param>
        public void Update(Rigidbody rigidBody)
        {
            this.rigidBody = rigidBody;
        }

        /// <summary>
        /// Extracts telemetry data from the rigidbody
        /// </summary>
        /// <param name="eulerType">how to calulate yaw pitch and roll</param>
        /// <returns></returns>
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

        public Vector3 GetPitchYawRoll(Vector3 eulerAngles)
        {
            var pyr = new Vector3(
                    Maths.HemiCircle(eulerAngles.x),
                    eulerAngles.y, //Maths.HemiCircle(eulerAngles.y),
                    Maths.HemiCircle(eulerAngles.z));

            return pyr;
        }

        private float ForwardVelocity()
        {
            return Mathf.Abs(Vector3.Dot(rigidBody.velocity, rigidBody.transform.forward));
        }

        public void Reset()
        {
            logger.LogWarning("Rigidbody is null, resetting yaw");
            _yaw = 0;
        }

    }

    internal struct BasicTelemetry
    {
        public Quaternion Rotation;
       
        public Vector3 EulerAngles;
        
        public Vector3 LocalAngularVelocity;        
        
        public Vector3 LocalVelocity;

        public Vector3 Accel;

        public float Speed;
        
        public float CentripetalForce;
        
    }
}

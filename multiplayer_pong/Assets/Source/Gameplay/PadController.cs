using Photon.Bolt;
using UnityEngine;

namespace MultiplayerPong.Gameplay
{
    public class PadController : Photon.Bolt.EntityBehaviour<IPadState>
    {
        public float Speed = 2f;

        public override void Attached()
        {
            base.Attached();
            state.SetTransforms(state.PadTransform, transform);
        }

        public override void SimulateOwner()
        {
            base.SimulateOwner();
            
            float y = Input.GetAxis("Vertical");
            transform.position += Vector3.up 
                                * y 
                                * Speed 
                                * BoltNetwork.FrameDeltaTime;
        }
    }
}
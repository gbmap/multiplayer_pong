using UnityEngine;
using Photon.Bolt;
using Zenject;
using System.Collections;

namespace MultiplayerPong.Gameplay
{
    public interface ISignalBusHolder
    {
        SignalBus SignalBus { get; set; }
    }

    public class BallBehaviour : Photon.Bolt.EntityBehaviour<IBallState>, ISignalBusHolder
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private float secondsToFreezeOnScore = 3f;
        private Vector3 velocity;

        public SignalBus SignalBus { get; set; }

        public override void Attached()
        {
            base.Attached();
            state.SetTransforms(state.BallTransform, transform);
        }

        void Start()
        {
            SignalBus?.Subscribe<SignalOnPlayerWin>(Callback_OnPlayerWin);
            RandomizeVelocity();
            FreezeForSeconds(5f);
        }

        private void Callback_OnPlayerWin()
        {
            Debug.Log("Game won, freezing ball.");
            FreezeForSeconds(float.MaxValue);
        }

        public override void SimulateOwner()
        {
            base.SimulateOwner();
            transform.position += velocity * speed * BoltNetwork.FrameDeltaTime;

            EScore score = EScore.None;
            BounceOffScreen(out score);

            if (score != EScore.None)
            {
                Score(score);

                RandomizeVelocity();
                transform.position = Vector3.zero;
                FreezeForSeconds(secondsToFreezeOnScore);
            }

        }

        private void RandomizeVelocity()
        {
            velocity = Random.insideUnitSphere.normalized;
            velocity.z = 0f;
        }

        private void BounceOffScreen(out EScore score)
        {
            Vector3 viewport = Camera.main.WorldToViewportPoint(transform.position);
            bool outOfScreenX = viewport.x <= 0f || viewport.x >= 1f;
            bool outOfScreenY = viewport.y <= 0f || viewport.y >= 1f;
            score = GetScore(viewport, outOfScreenX);

            AdjustPosition(viewport);

            if (outOfScreenY)
                velocity.y *= -1;
        }

        private void AdjustPosition(Vector3 viewport)
        {
            float epsilon = 0.01f;

            if (viewport.x <= 0f)
                viewport.x = epsilon;
            else if (viewport.x >= 1f)
                viewport.x = 1f - epsilon;

            if (viewport.y <= 0f)
                viewport.y = epsilon;
            else if (viewport.y >= 1f)
                viewport.y = 1f - epsilon;

            transform.position = Camera.main.ViewportToWorldPoint(viewport);
        }

        private EScore GetScore(
            Vector3 viewport, 
            bool isOutOfScreenX
        ) {
            if (isOutOfScreenX)
            {
                if (viewport.x <= 0f)
                    return EScore.Player2Scored;
                else if (viewport.x >= 1f)
                    return EScore.Player1Scored;

            }
            return EScore.None;
        }

        void Score(EScore score)
        {
            Debug.Log("[Ball] Player scored.");
            SignalBus.Fire<SignalOnPlayerScored>(new SignalOnPlayerScored(score));
        }

        void FreezeForSeconds(float time)
        {
            StartCoroutine(FreezeForSecondsCoroutine(time));
        }

        IEnumerator FreezeForSecondsCoroutine(float time)
        {
            float t = 0f;
            while (t < time)
            {
                transform.position = Vector3.zero;
                yield return null;
                t += Time.deltaTime;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Pad"))
            {
                Debug.Log("Collided with player.");
                velocity.x *= -1;
            }
        }
    }
}
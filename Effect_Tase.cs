using UnityEngine;
using Landfall.TABS;
using System.Collections;

namespace Creative
{
    public class Effect_Tase : UnitEffectBase
    {
        public override void DoEffect()
        {
            Tase(taseAmount);
        }

        public override void Ping()
        {
            Tase(taseAmount);
        }

        public void Tase(float amount)
        {
            StartCoroutine(DoTase(amount));
        }

        public IEnumerator DoTase(float amount)
        {
            if (transform.root.GetComponent<Unit>().data.GetComponent<StandingHandler>())
            {
                //Credit to Harren Tonderen for helping me figure out how to do this.
                transform.root.GetComponent<Unit>().data.GetComponent<StandingHandler>().selfOffset -= selfOffset;
            }
            if (transform.root.GetComponentInChildren<StandingBodyPart>())
            {
                foreach (var stand in transform.root.GetComponentsInChildren<StandingBodyPart>())
                {
                    stand.enabled = false;
                }
            }
            taserEffect = amount;
            yield return new WaitUntil(() => taserEffect <= 0f);
            Debug.Log("Fridge");
            if (transform.root.GetComponent<Unit>().data.GetComponent<StandingHandler>())
            {
                //Credit to Harren Tonderen for helping me figure out how to do this.
                transform.root.GetComponent<Unit>().data.GetComponent<StandingHandler>().selfOffset += selfOffset;
            }
            if (transform.root.GetComponentInChildren<StandingBodyPart>())
            {
                foreach (var stand in transform.root.GetComponentsInChildren<StandingBodyPart>())
                {
                    stand.enabled = false;
                }
            }
            yield break;
        }

        public void FixedUpdate()
        {
            if (taserEffect > 0f && !transform.root.GetComponent<Unit>().data.Dead)
            {
                taserEffect -= Time.fixedDeltaTime;
                for (int i = 0; i < transform.root.GetComponent<Unit>().data.GetComponent<RigidbodyHolder>().AllRigs.Length; i++)
                {
                    Rigidbody rigidbody = transform.root.GetComponent<Unit>().data.GetComponent<RigidbodyHolder>().AllRigs[i];
                    rigidbody.AddTorque(Random.insideUnitSphere * 100f * Mathf.Cos(Time.time * 15f), ForceMode.Force);
                    rigidbody.AddTorque(Random.insideUnitSphere * 2000f * Mathf.Cos(Time.time * 15f), ForceMode.Acceleration);
                }
            }
        }

        private float taserEffect;

        public float taseAmount = 4f;

        public float selfOffset = 2f;
    }
}

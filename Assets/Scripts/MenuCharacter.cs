    using System;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public class MenuCharacter : MonoBehaviour
    {
        [NonSerialized]
        public Vector2 targetPosition;
        public GameObject beamPrefab;
        private bool atEnd = false;
        private float endTimer = 0;
        private void Update()
        {

            if (atEnd)
            {
                endTimer += Time.deltaTime;
                if (endTimer > .4f)
                {
                    Destroy(gameObject);
                    Instantiate(beamPrefab, transform.position, Quaternion.identity);
                }
            }
            else
            {
                var dir = targetPosition - (Vector2)transform.position;
                transform.Translate(dir.normalized * 1 * Time.deltaTime);
            
                var dist = Vector2.Distance(transform.position, targetPosition);

                if (dist < .3f)
                {
                    atEnd = true;
                }
            }
        }
    }

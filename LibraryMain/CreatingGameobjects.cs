using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.Components.Animation;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace BaldiTestMod
{
    public static class CreateGameobject
    {
        public static GameObject CreateLaserBeam(Vector3 startPosition, Vector3 direction, float length, Color color)
        {
            GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cube);

            beam.transform.position = startPosition;

            beam.transform.rotation = Quaternion.LookRotation(direction);

            beam.transform.localScale = new Vector3(0.1f, 0.1f, length);

            beam.transform.position = startPosition + direction * (length / 2f);

            beam.layer = LayerMask.NameToLayer("Billboard");

            Renderer renderer = beam.GetComponent<Renderer>();
            renderer.material.color = color;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.material.shader = Shader.Find("Unlit/Color");

            Collider col = beam.GetComponent<Collider>();
            if (col != null) GameObject.Destroy(col);

            GameObject.Destroy(beam, 1f);

            return beam;
        }
        public static GameObject CreateBullet(Vector3 startPosition, Vector3 direction, float speed, Color color)
        {
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bullet.transform.position = startPosition;
            bullet.transform.rotation = Quaternion.LookRotation(direction);
            bullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.5f);
            Renderer renderer = bullet.GetComponent<Renderer>();
            bullet.layer = LayerMask.NameToLayer("Billboard");
            renderer.material.color = color;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.material.shader = Shader.Find("Unlit/Color");

            BulletMover mover = bullet.AddComponent<BulletMover>();
            mover.speed = speed;
            mover.direction = direction;


            return bullet;
        }
        public class BulletMover : MonoBehaviour
        {
            public float speed = 15f;
            public Vector3 direction;

            void Update()
            {
                transform.position += direction * speed * Time.deltaTime;
            }
        }
    }
}
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
        public static GameObject CreateBullet(EnvironmentController ec, Vector3 startPosition, Vector3 direction, float speed, Color color, Baldi baldi)
        {
            GameObject bullet = new GameObject("Bullet");
            bullet.transform.position = startPosition;
            bullet.transform.rotation = Quaternion.LookRotation(direction);

            BulletComponent bulletComp = bullet.AddComponent<BulletComponent>();
            bulletComp.Initialize(direction, speed, baldi);

            GameObject collisionChild = GameObject.CreatePrimitive(PrimitiveType.Cube);
            collisionChild.name = "Collision";
            collisionChild.transform.SetParent(bullet.transform);
            collisionChild.transform.localPosition = Vector3.zero;
            collisionChild.transform.localRotation = Quaternion.LookRotation(direction);
            collisionChild.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            collisionChild.layer = LayerMask.NameToLayer("Ignore Raycast");

            BulletCollisionProxy proxy = collisionChild.AddComponent<BulletCollisionProxy>();
            proxy.owner = bulletComp;

            Renderer collisionRenderer = collisionChild.GetComponent<Renderer>();
            if (collisionRenderer != null)
            {
                GameObject.Destroy(collisionRenderer);
            }

            var collider = collisionChild.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            GameObject visualChild = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visualChild.name = "Visual";
            visualChild.transform.SetParent(bullet.transform);
            visualChild.transform.localPosition = Vector3.zero;
            visualChild.transform.localRotation = Quaternion.LookRotation(direction);
            visualChild.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            visualChild.layer = LayerMask.NameToLayer("Billboard");

            Renderer visualRenderer = visualChild.GetComponent<Renderer>();
            visualRenderer.material.color = color;
            visualRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            visualRenderer.receiveShadows = false;
            visualRenderer.material.shader = Shader.Find("Unlit/Color");

            Collider visualCollider = visualChild.GetComponent<Collider>();
            if (visualCollider != null)
            {
                GameObject.Destroy(visualCollider);
            }

            GameObject.Destroy(bullet, 10f);

            return bullet;
        }
    }
}
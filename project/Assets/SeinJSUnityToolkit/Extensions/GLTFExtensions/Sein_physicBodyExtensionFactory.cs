﻿/**
 * @File   : Sein_physicBodyExtensionFactory.cs
 * @Author : dtysky (dtysky@outlook.com)
 * @Link   : dtysky.moe
 * @Date   : 2019/10/12 0:00:00AM
 */
using System;
using Newtonsoft.Json.Linq;
using GLTF.Math;
using Newtonsoft.Json;
using GLTF.Extensions;
using System.Collections.Generic;
using UnityEngine;
using GLTF.Schema;

namespace SeinJS
{
    public class Sein_physicBodyExtensionFactory : SeinExtensionFactory
    {
        public override string GetExtensionName() { return "Sein_physicBody"; }
        public override List<Type> GetBindedComponents() { return new List<Type> { typeof(SeinRigidBody), typeof(Collider) }; }

        public override void Serialize(ExporterEntry entry, Dictionary<string, Extension> extensions, UnityEngine.Object component = null)
        {
            Sein_physicBodyExtension extension = null;

            if (extensions.ContainsKey(ExtensionName))
            {
                extension = (Sein_physicBodyExtension)extensions[ExtensionName];
            }
            else
            {
                extension = new Sein_physicBodyExtension();
                AddExtension(extensions, extension);
            }

            if (component is SeinRigidBody)
            {
                extension.rigidBody = component as SeinRigidBody;
            }
            else if (component is Collider)
            {
                if (extension.colliders == null)
                {
                    extension.colliders = new List<Collider>();
                }

                extension.colliders.Add(component as Collider);
            }
        }

        public override Extension Deserialize(GLTFRoot root, JProperty extensionToken)
        {
            var extension = new Sein_physicBodyExtension();
            SeinRigidBody rigidBody = null;
            List<Collider> colliders = new List<Collider>();

            var tmpGo = new GameObject();

            rigidBody = tmpGo.AddComponent<SeinRigidBody>();
            rigidBody.mass = (float)extensionToken.Value["mass"];
            rigidBody.friction = (float)extensionToken.Value["friction"];
            rigidBody.restitution = (float)extensionToken.Value["restitution"];
            rigidBody.unControl = (bool)extensionToken.Value["unControl"];
            rigidBody.physicStatic = (bool)extensionToken.Value["physicStatic"];
            rigidBody.sleeping = (bool)extensionToken.Value["sleeping"];

            foreach (JContainer collider in extensionToken.Value["colliders"]) {
                var type = (string)collider["type"];

                switch (type)
                {
                    case ("SPHERE"):
                        var sc = tmpGo.AddComponent<SphereCollider>();
                        sc.radius = (float)collider["radius"];
                        sc.center = new  UnityEngine.Vector3(
                            (float)collider["offset"][0],
                            (float)collider["offset"][1],
                            (float)collider["offset"][2]
                        );
                        sc.isTrigger = (bool)collider["isTrigger"];

                        colliders.Add(sc);
                        break;
                    case ("BOX"):
                        var bc = tmpGo.AddComponent<BoxCollider>();
                        bc.size = new UnityEngine.Vector3(
                            (float)collider["size"][0],
                            (float)collider["size"][1],
                            (float)collider["size"][2]
                        );
                        bc.center = new UnityEngine.Vector3(
                            (float)collider["offset"][0],
                            (float)collider["offset"][1],
                            (float)collider["offset"][2]
                        );
                        bc.isTrigger = (bool)collider["isTrigger"];

                        colliders.Add(bc);
                        break;
                    default:
                        Debug.LogWarning("In current time, Sein only supports shpere and box collider !");
                        break;
                }
            }

            extension.rigidBody = rigidBody;
            extension.colliders = colliders;
            extension.tmpGo = tmpGo;

            return extension;
        }
    }
}
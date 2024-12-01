using Blocks.Sockets;
using ElasticSea.Framework.Extensions;
using UnityEngine;

namespace Blocks.Templates.Strategies
{
    public abstract class TemplateStrategy : ITemplateFactory
    {
        private Mesh pinMesh;

        protected TemplateStrategy(Mesh pinMesh)
        {
            this.pinMesh = pinMesh;
        }

        public abstract void Build(Block block);

        protected void AddSocket(Block owner, SocketType type, Vector3 offset, int width, int height)
        {
            for (var x = 0; x < width; x++)
            {
                for (var z = 0; z < height; z++)
                {
                    var pin = new GameObject($"{(type == SocketType.Male ? "Male" : "Female")} [{x}][{z}]");
                    var socket = pin.AddComponent<Socket>();
                    socket.Type = type;
                    socket.Block = owner;
                    pin.transform.SetParent(owner.transform, false);
                    if (type == SocketType.Female)
                    {
                        pin.transform.localRotation = Quaternion.Euler(180, 0, 0);
                    }
                    else
                    {
                        var pinMf = pin.AddComponent<MeshFilter>();
                        pinMf.sharedMesh = pinMesh;

                        var pinMr = pin.AddComponent<MeshRenderer>();
                    }

                    var offset2 = new Vector3(x + .5f, 0, z + .5f);
                    pin.transform.position = offset + offset2.Multiply(new Vector3(0.05f, 1, 0.05f)).RoundTo(0.05f, 1, 0.05f);
                    owner.AddSocket(socket);
                }
            }
        }
    }
}
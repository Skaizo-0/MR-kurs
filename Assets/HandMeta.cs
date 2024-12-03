using Blocks.Builder;
using Blocks;
using System;
using System.Linq;
using UnityEngine;

namespace Sandbox.Controller
{
    public class HandMeta : MonoBehaviour
    {
        private bool isGrabbing;  // ����, ����������� �� ��, ��� ������ � ������ ������ ��������
        private Chunk chunkHeld;  // ������� ����������� ����
        [SerializeField] private ChunkSpawner chunkSpawner;  // ������� ������
        private Helper helper;  // ��������������� ��������� ��� ������ � �������������

        // ������������ ���������, ���������� ������ Grab � Disconnect
        public enum State
        {
            Grab, Disconnect
        }

        [SerializeField] private State state;  // ������� ���������

        // �����, ���������� ��� ������� �������
        private void Awake()
        {
            // � ���� ������ ��� ������������� � ������������, ����������� �� ��� ������������ ������ �������� ����
            helper = gameObject.AddComponent<Helper>();  // ��������� ��������� Helper ��� ���������� �������������
        }

        // ����� ��� ���������� ��������� ������ ����
        private void Update()
        {
            // ���������, �������� �� ����
            if (chunkHeld)
            {
                var component = chunkHeld.GetComponent<Rigidbody>();  // �������� Rigidbody ��� ���������� ������� �������
                component.isKinematic = true;  // ��������� ������ ��� ��������� �������
                component.transform.position = transform.position;  // ����������� ������ � ����
                component.transform.rotation = transform.rotation;  // ����������� ������� ������� � �������� ����
            }

            // �������� �� ����������� ���� � ������� ��� �������
            if (IsHandNearObject())
            {
                TryGrabObject();
            }

            // �������� �� ���������� ����� "���������" (��������, ���� ������������� ��� ������� �� �������)
            if (isGrabbing && !IsHandNearObject())
            {
                ReleaseObject();
            }

            UpdateState();  // ��������� ��������� � ����������� �� ��������
        }

        // ��������, ��������� �� ���� ����� � ��������, ������� ����� ���������
        private bool IsHandNearObject()
        {
            // ��������� ��� ������� � ������� 0.05 �� ������� ����
            var nearbyObjects = Physics.OverlapSphere(transform.position, 0.05f);
            return nearbyObjects.Any(c => c.GetComponent<Block>());  // ���� ���� ���� �� ���� ������ ���� Block, �������, ��� ���� �����
        }

        // ������� ������� �������, ���� ���� ����� � ���
        private void TryGrabObject()
        {
            if (isGrabbing || chunkHeld != null) return;  // ���� ��� �������� ������, ������ �� ������

            var blockCandidate = CheckForChunk();  // ���� ���������� ������ ��� �������

            if (blockCandidate)
            {
                chunkHeld = blockCandidate;  // ����������� ��������� ����
                chunkHeld.GetComponent<BuildPreviewManager>().StartPreview();  // ��������� ��������������� ��������
                isGrabbing = true;  // ��������� ���� �������
            }
        }

        // ����� ��� ���������� �������
        private void ReleaseObject()
        {
            if (chunkHeld)
            {
                chunkHeld.GetComponent<Rigidbody>().isKinematic = false;  // �������� ������ ��� �������
                chunkHeld.GetComponent<BuildPreviewManager>().StopPreview();  // ������������� ��������������� ��������
            }

            chunkHeld = null;  // ���������� ����������� ����
            isGrabbing = false;  // ���������� ���� �������
        }

        // ����� ��� ���������� ���������
        private void UpdateState()
        {
            switch (state)
            {
                case State.Grab:
                    // ������ ��� ��������� "Grab" (������)
                    break;
                case State.Disconnect:
                    // ������ ��� ��������� "Disconnect" (����������)
                    break;
                default:
                    throw new ArgumentOutOfRangeException();  // ������ ��� ����������� ���������
            }
        }

        // ����� ��� ������ ���������� ����� ��� �������
        private Chunk CheckForChunk()
        {
            // ���� ��� ������� � ������� 0.05 �� ����
            var blockCandidate = Physics.OverlapSphere(transform.position, 0.05f)
                .Where(c => c.GetComponent<Block>())  // ���� ������ ������� � ����������� Block
                .Where(c => c.GetComponent<Block>().IsAnchored == false)  // ��������� �������������� �����
                .OrderBy(c => Vector3.Distance(c.transform.position, transform.position))  // ��������� �� ��������� � ����
                .FirstOrDefault();  // ����� ������ (����� �������)

            if (blockCandidate)
            {
                var chunk = blockCandidate.GetComponentInParent<Chunk>();  // �������� ����
                if (chunk.GetComponent<Rigidbody>().isKinematic == false)  // ���������, �� �������� �� ���� ��������������
                {
                    return chunk;  // ���������� ����
                }
            }

            return null;  // ���� ���� �� ������, ���������� null
        }
    }
}

using UnityEngine;

public class AlternatingBlockController : MonoBehaviour
{
    private enum MoveState
    {
        ToEnd,
        BackToStart
    }

    [Header("Blocks")]
    [SerializeField] private Transform _block1;
    [SerializeField] private Transform _block2;

    [Header("Move Points")]
    [SerializeField] private Transform _startPoint; // Z lớn hơn
    [SerializeField] private Transform _endPoint;   // Z nhỏ hơn

    [Header("Move Data")]
    [SerializeField] private float _speed = 4f;
    [SerializeField] private float _switchDelay = 0.5f;

    private Transform _currentBlock;
    private Transform _nextBlock;

    private float _zStart; // lớn hơn
    private float _zEnd;   // nhỏ hơn

    private MoveState _state;

    private bool _isWaiting;
    private float _waitTimer;

    private void Start()
    {
        // ÉP CHUẨN: Start luôn lớn hơn End
        _zStart = Mathf.Max(_startPoint.position.z, _endPoint.position.z);
        _zEnd = Mathf.Min(_startPoint.position.z, _endPoint.position.z);

        _block1.gameObject.SetActive(false);
        _block2.gameObject.SetActive(false);

        _currentBlock = _block1;
        _nextBlock = _block2;

        SpawnAtStart(_currentBlock);
    }

    private void Update()
    {
        if (_isWaiting)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _switchDelay)
            {
                _isWaiting = false;
                _waitTimer = 0f;
                DoSwitch();
            }
            return;
        }

        MoveBlock();
    }

    private void MoveBlock()
    {
        Vector3 pos = _currentBlock.position;
        float step = _speed * Time.deltaTime;

        if (_state == MoveState.ToEnd)
        {
            pos.z -= step;

            if (pos.z <= _zEnd)
            {
                pos.z = _zEnd;
                _state = MoveState.BackToStart;
            }
        }
        else // BackToStart
        {
            pos.z += step;

            if (pos.z >= _zStart)
            {
                pos.z = _zStart;
                _currentBlock.position = pos;
                BeginSwitch();
                return;
            }
        }

        _currentBlock.position = pos;
    }

    private void SpawnAtStart(Transform block)
    {
        Vector3 pos = block.position;
        pos.z = _zStart;
        block.position = pos;

        block.gameObject.SetActive(true);
        _state = MoveState.ToEnd;
    }

    private void BeginSwitch()
    {
        _currentBlock.gameObject.SetActive(false);
        _isWaiting = true;
        _waitTimer = 0f;
    }

    private void DoSwitch()
    {
        Transform temp = _currentBlock;
        _currentBlock = _nextBlock;
        _nextBlock = temp;

        SpawnAtStart(_currentBlock);
    }
}

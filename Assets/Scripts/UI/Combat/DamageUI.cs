using TMPro;
using UnityEngine;
using DG.Tweening;

namespace UI.Combat
{
    public class DamageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshPro text;
        [SerializeField] private float lifeTime = 1f;
        [Tooltip("Damage word going up to certain distance")]
        [SerializeField] private float maxDistance = 5f;
        [SerializeField] private float offset = 5f;

        private Vector3 _initialPos;
        private Vector3 _initialScale;

        private void Start()
        {
            if(text == null) text = GetComponent<TextMeshPro>();
            _initialScale = transform.localScale;
        }

        public void Play(Vector3 initialPos, int damage = 0)
        {
            transform.LookAt(2f * transform.position - Camera.main.transform.position);
            transform.position = new Vector3(initialPos.x, initialPos.y + offset, initialPos.z);
            _initialPos = transform.position;
            text.text = damage.ToString();

            Vector3 move = _initialPos;
            move.y += maxDistance;
            transform.DOMove(move, lifeTime).OnComplete(() => Destroy(gameObject));
            transform.DOScale(_initialScale * 1.5f, lifeTime);
            text.DOColor(Color.clear, lifeTime);
        }

        //[Header("Configuration")]
        //[SerializeField] private TextMeshProUGUI text;
        //[SerializeField] private float lifeTime = 0.6f;
        //[SerializeField] private float minDistance = 2f;
        //[SerializeField] private float maxDistance = 3f;

        //private Vector3 _initialPos;
        //private Vector3 _targetPos;
        //private float _timer;

        //private void Start()
        //{
        //    transform.LookAt(2f * transform.position - Camera.main.transform.position);

        //    float direction = Random.rotation.eulerAngles.z;
        //    _initialPos = transform.position;
        //    float distance = Random.Range(minDistance, maxDistance);
        //    _targetPos = _initialPos + (Quaternion.Euler(0f, 0f, direction) * new Vector3(distance, distance, 0f));
        //    transform.localScale = Vector3.zero;
        //}

        //private void Update()
        //{
        //    _timer += Time.deltaTime;

        //    float fraction = lifeTime / 2f;
        //    if (_timer > lifeTime) Destroy(gameObject);
        //    else if(_timer > fraction)
        //    {
        //        text.color = Color.Lerp(text.color, Color.clear, (_timer - fraction) / (lifeTime - fraction));
        //    }

        //    transform.localPosition = Vector3.Lerp(_initialPos, _targetPos, Mathf.Sin(_timer / lifeTime));
        //    transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, Mathf.Sin(_timer / lifeTime));
        //}

        //public void SetText(int damage)
        //{
        //    text.text = damage.ToString();
        //}
    }

}
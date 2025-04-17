using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class Ascender : MonoBehaviour
{
    private Animator _animador;
    private void Awake()
    {
        _animador = GetComponent<Animator>();
    }

    private void Update()
    {
        AnimatorStateInfo _info = _animador.GetCurrentAnimatorStateInfo(0);
        if (_info.IsName("ascension"))
            if (_info.normalizedTime >= 1.0f)
                Destroy(transform.parent.gameObject);
    }

    // ----------( Funciones de Debug )---------- //
    private void OnDrawGizmos()
    {
        #if UNITY_EDITOR
        {
            Handles.color = Color.red;
            Handles.Label(transform.position + Vector3.up * 0.5f,
                                    $"ASCENDER : tiempo -> posicion -> {transform.position}");
        }
        #endif
    }
}

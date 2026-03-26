using Fusion;
using UnityEngine;

public class SimpleBullet : NetworkBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float hitRadius = 0.3f;

    [Networked] private TickTimer LifeTimer { get; set; }  //네트워크의 타이머

    [Networked] private PlayerRef Owner {  get; set; }

    public void Init(PlayerRef owner)
    {
        Owner = owner;
    }
    public override void Spawned()  //네트워크상 스폰이 되었을때
    {
        if (Object.HasStateAuthority)  //오브젝트의 권한이 있을때
        {
            LifeTimer = TickTimer.CreateFromSeconds(Runner, lifeTime);  //타이머를 세팅함
        }
    }
    public override void FixedUpdateNetwork()
    {
        transform.position += transform.forward * speed * Runner.DeltaTime;

        if (Object.HasStateAuthority)
        {
            if (LifeTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
                return;
            }

            Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);
            foreach (var hit in hits)
            {
                SimplePlayer player = hit.GetComponentInParent<SimplePlayer>();

                if (player == null) continue;
                if (player.Object.InputAuthority == Owner) continue;

                Debug.Log($"총알이 플레이어를 맞춤 : {player.Object.InputAuthority}");

                Runner.Despawn(Object);
                return;
            }
        }
    }

}

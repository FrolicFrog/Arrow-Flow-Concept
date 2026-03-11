using ArrowFlowGame.Types;
using DG.Tweening;
using TMPro;

public class Giant: Person
{
    public TextMeshPro HitsRequiredLabel;

    public override void Init(CrowdElementData crowdElement)
    {
        base.Init(crowdElement);
        HitsRequiredLabel.text = RequiredHits.ToString();
    }

    public override void Damage()
    {
        base.Damage();
        HitsRequiredLabel.text = RequiredHits.ToString();
        
        if (RequiredHits > 0)
        {
            Sequence DeathSequence = DOTween.Sequence();

            if(!Anim.GetCurrentAnimatorStateInfo(0).IsName("Hit 1") || !Anim.GetCurrentAnimatorStateInfo(0).IsName("Hit 2"))
            Anim.Play("Hit " + UnityEngine.Random.Range(1,3));
            DeathSequence.AppendCallback(() => SwitchMaterial(ReferenceManager.Instance.DamageFlashedPerson));
            DeathSequence.AppendCallback(() => DamageEffect.Play());
            DeathSequence.InsertCallback(0.25f, () => SwitchMaterial(ReferenceManager.Instance.ItemMats.GetMaterial(Type)));
        }
        else
        {
            DamageEffect.Play();
        }
    }
    
    protected override void Dead()
    {
        HitsRequiredLabel.gameObject.SetActive(false);
        Anim.Play("ZombieDeath");
        SwitchMaterial(ReferenceManager.Instance.DeadPersonMaterial);
        Destroy(gameObject, 2.3f);
    }
}
using ArrowFlowGame.Types;
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
    }

    protected override void Dead()
    {
        base.Dead();
        HitsRequiredLabel.gameObject.SetActive(false);
    }
}
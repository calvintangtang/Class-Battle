using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour {
    public Image hpBarImage;
    public Image mpBarImage;
    public SpriteRenderer hpBar;
    public CharacterCombat charComb
    {
        get { return _charComb; }
        set {
            _charComb = value;
            _charComb.getHurtAction += hpChange;
            _charComb.onDeathAction += hpChange;
        }
    }

    public CharacterCombat _charComb;

	void Start() {
        if (_charComb != null)
            charComb = _charComb;
    }
	
	void LateUpdate() {
        if (hpBar != null) {
            Vector3 scale = this.transform.localScale;
            scale.x = (charComb.transform.localScale.x < 0) ? -1 : 1;
            this.transform.localScale = scale;
        }

        if (mpBarImage != null && charComb != null) {
            mpBarImage.fillAmount = 1.0f * charComb.stats.curMP / charComb.stats.maxMP;
        }
        hpChange();
    }

    public void hpChange() {
        if (charComb == null) {
            return;
        }
        if (hpBar != null)
        {
            Vector3 scale = hpBar.transform.localScale;
            scale.x = 1.0f * charComb.stats.curHP / charComb.stats.maxHP;
            hpBar.transform.localScale = scale;
        }
        if(hpBarImage != null)
        {
            hpBarImage.fillAmount = 1.0f * charComb.stats.curHP / charComb.stats.maxHP;
        }
    }
}

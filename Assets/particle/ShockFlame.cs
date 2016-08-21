using UnityEngine;
using System.Collections;

public class ShockFlame : MonoBehaviour {
    public ParticleSystem shockFlame;
    public ParticleSystem flame;

    public void SetColor(int gemno)
    {
        switch (gemno)
        {
            case 1:
                SetColor(Color.red);
                break;
            case 2:
                SetColor(Color.green);
                break;
            case 3:
                SetColor(Color.blue);
                break;
            case 4:
                SetColor(Color.yellow);
                break;
            case 5:
                SetColor(Color.blue);
                break;

        }
    }

    void SetColor(Color color)
    {
        shockFlame.startColor = color;
        flame.startColor = color;
    }
}

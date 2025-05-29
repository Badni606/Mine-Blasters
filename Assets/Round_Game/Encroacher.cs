using UnityEngine;

public class Encroacher : MonoBehaviour
{
    public Transform BlacknessTransform;
    public Transform SpriteMaskTransform;
    

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !RoundManager.instance.isGameOver)
        {
            collision.GetComponent<Player>().TakeDamange(10); //TODO Make use max health
        }
        else if (collision.CompareTag("PickUp")){
            collision.GetComponent<PickUp>().DestoryPickUp();
        }
    }

    float maxScale;
    public float DurationOfEncroach;
    float currentDurationLeft = 0f;


    public void Init(Vector2 size)
    {
        //set size of Black background
        BlacknessTransform.position = size * new Vector2(0.5f,0.5f); //new pivot
        BlacknessTransform.localScale = size;   //expand

        SpriteMaskTransform.position = size * new Vector2(0.5f, 0.5f); //nwe pivot, again....
        maxScale = size.x + (size.x - size.y);
        SpriteMaskTransform.localScale = new Vector2(maxScale,maxScale); //need it a bit bigger so it doesn't cut off corners

        circleCollider = GetComponent<CircleCollider2D>();
        circleCollider.radius = maxScale /2 ;
        circleCollider.enabled = false;//just to make sure
        //maxScale = size.x;
    }


    CircleCollider2D circleCollider;
    public void BeginEncroach(Vector2 targetLocation)
    {
        circleCollider.offset = targetLocation;
        circleCollider.enabled = true;

        SpriteMaskTransform.position = targetLocation;
        currentDurationLeft = DurationOfEncroach;
    }

    private void FixedUpdate()
    {
        if (currentDurationLeft > 0)
        {
            currentDurationLeft -= Time.deltaTime;
            float newDiameter = Mathf.Lerp(0, maxScale, currentDurationLeft / DurationOfEncroach);
            circleCollider.radius = newDiameter / 2f;
            SpriteMaskTransform.localScale = new Vector3(newDiameter, newDiameter, 1);
            //gameObject.transform.localScale = new Vector3(Mathf.Lerp(0, maxScale,currentDurationLeft/DurationOfEncroach),1,1);
        }

    }

    public void Stop()
    {
        currentDurationLeft=0;
    }
}

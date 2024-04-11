//计时器

public class CDTimer
{
    private float m_duration;
    public float Duration
    {
        get { 
            return m_duration; 
        }
    }

    private float m_current;
    public float Current
    {
        get { 
            return m_current; 
        }
    }

    public CDTimer(float duration)
    {
        m_current = 0;
        m_duration = duration;
    }

    public void Full()
    {
        m_current = m_duration;
    }

    public void Reset()
    {
        m_current = 0;
    }

    public void Reset(float duration)
    {
        m_current = 0;
        m_duration = duration;
    }

    public void SetDuration(float duration)
    {
        m_duration = duration;
    }

    public void SetCurrent(float value)
    {
        m_current = value;
    }

    public bool IsFinished()
    {
        return m_current >= m_duration;
    }

    public void Update(float deltatime) 
    {
        m_current += deltatime;
    }
}

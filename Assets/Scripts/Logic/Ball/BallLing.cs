using UnityEngine;

namespace CB
{
    /// <summary>
    /// 菱形弹珠
    /// </summary>
    /// 
    public class BallLing : Ball
    {
        private int m_Rate;
        private int m_Order = 2;


        public override void UpgradeTo(int level)
        {
            base.UpgradeTo(level);

            m_Rate = 30 + 5 * m_Level;
        }



        public override string GetDescription()
        {
            var str = string.Format("击中宝石后有<size=32><#43A600>{0}%</color></size>的概率将当前宝石转换成<sprite=5>", m_Rate);

            return str;
        }

        //碰撞逻辑
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            this.CancelIgnoreCollision();
            this.OnHitGhost(collision);
            this.OnHitObstable(collision);

            Obstacle obt = collision.transform.GetComponent<Obstacle>();
            if (obt != null && obt.HasShield() == false) {
                if (obt.IsDead() == false && obt.Order != m_Order) {
                    if (RandomUtility.IsHit(m_Rate) == true) {
                        GameFacade.Instance.EffectManager.Load(EFFECT.SMOKE, obt.transform.localPosition);
                        
                        var copy = GameFacade.Instance.Game.PushObstacle(obt.transform.localPosition, obt.HP, m_Order);
                        copy.CopyChanges(obt);
                        copy.AddChange(m_Order);
                        if (copy.IsChangeFull() == true) {
                            copy.OnHit(this, copy.HP);
                        }

                        obt.ForceDead();
                    }
                }
            }
        }
    }
}
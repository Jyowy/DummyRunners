using Common;
using Unity.Mathematics;

namespace Player
{

    [System.Serializable]
    public class Wallet
    {

        public float CurrentMoney { get; private set; } = 0f;

        public string FormattedMoney => GameUtils.GetMoneyFormatted(CurrentMoney);

        public Wallet() { }

        public Wallet(Wallet other)
        {
            if (other == null)
            CurrentMoney = other.CurrentMoney;
        }

        public bool HasEnoughMoney(float money)
            => money <= CurrentMoney;

        public bool Spend(float money)
        {
            bool canSpend = HasEnoughMoney(money);

            if (canSpend)
            {
                CurrentMoney -= math.max(money, 0f);
            }

            return canSpend;
        }

        public void Receive(float money)
        {
            CurrentMoney += math.max(money, 0f);
        }

    }

}
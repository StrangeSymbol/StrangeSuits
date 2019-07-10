namespace StrangeSuits
{
    interface IHand
    {
        void AddCardToHand(CardSprite card);
        void RemoveCardFromHand(int index);
        Microsoft.Xna.Framework.Vector2 GetNextHandPosition();
    }
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StrangeSuits
{
    public enum RankValue { Ace, Deuce, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, }
    public enum Suit { Club = 0, Spades, Hearts, Diamond }

    public class CardSprite : Sprite
    {
        private Vector2 direction;
        private double time;
        private int percent;
        private bool isFromFaceDownToFaceUp = true;
        private bool isFaceDown;
        const float speed = 1000f;

        Rectangle destination;

        private RankValue rank;
        private Suit cardSuit;

        public CardSprite(Texture2D texture, Texture2D overlay, RankValue rank, Suit cardSuit)
            : this(texture, overlay, Vector2.Zero, rank, cardSuit)
        {
        }

        public CardSprite(Texture2D texture, RankValue rank, Suit cardSuit)
            : this(texture, null, Vector2.Zero, rank, cardSuit)
        {
        }

        public CardSprite(Texture2D texture, Texture2D overlay, Vector2 position, RankValue rank, Suit cardSuit)
            : base(texture, position, overlay)
        {
            this.rank = rank;
            this.cardSuit = cardSuit;
            destination = CollisionRectangle;
        }

        public CardSprite(Texture2D texture)
            : this(texture, null, RankValue.Ace, Suit.Club)
        {
        }

        public CardSprite(Texture2D texture, Vector2 pos)
            : this(texture, null, pos, RankValue.Ace, Suit.Club)
        {
        }

        public RankValue Rank { get { return rank; } set { rank = value; } }
        public Suit CardSuit { get { return cardSuit; } set { cardSuit = value; } }
        public bool IsFromFaceDownToFaceUp { get { return IsFromFaceDownToFaceUp; } set { isFromFaceDownToFaceUp = value; } }
        public Rectangle Destination { set { destination = value; } }
        public double Time { get { return time; } }
        public bool IsMoving { get; set; }
        public bool GotToDestination { get; set; }
        

        public void CourseToCard(CardSprite card)
        {
            Vector2 p1 = new Vector2(this.position.X + this.sprite.Width / 2, this.position.Y + this.sprite.Height / 2);
            Vector2 p2 = new Vector2(card.position.X + card.sprite.Width / 2, card.position.Y + card.sprite.Height / 2);
            direction = Vector2.Normalize(new Vector2(p2.X - p1.X, p2.Y - p1.Y));
            double distance = Vector2.Distance(p1, p2);
            time = distance / speed * 1000;
        }

        public void CourseToCard(Vector2 p2)
        {
            Vector2 p1 = new Vector2(this.position.X + this.sprite.Width / 2, this.position.Y + this.sprite.Height / 2);
            direction = Vector2.Normalize(new Vector2(p2.X + this.sprite.Width / 2 - p1.X, p2.Y + this.sprite.Height / 2 - p1.Y));
            double distance = Vector2.Distance(p1, new Vector2(p2.X + this.sprite.Width / 2, p2.Y + this.sprite.Height / 2));
            time = distance / speed * 1000;
        }

        private double distanceToDestination(Texture2D sprite, Vector2 position, Vector2 p2)
        {
            Vector2 p1 = new Vector2(position.X + sprite.Width / 2, position.Y + sprite.Height / 2);
            return Vector2.Distance(p1, new Vector2(p2.X + sprite.Width / 2, p2.Y + sprite.Height / 2));
        }

        public void Move(GameTime gameTime)
        {
            position += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds) * direction;
        }

        public void Move(GameTime gameTime, Vector2 initial, Vector2 final)
        {
            position += (float)(speed * gameTime.ElapsedGameTime.TotalSeconds) * direction;
            double distanceFromInitToFinal = distanceToDestination(this.sprite, initial, final);
            double distanceFromPosToDest = distanceToDestination(this.sprite, position, final);
            // Calculate the completion percent of the animation
            percent = 100 - (int)(distanceFromPosToDest / distanceFromInitToFinal * 100);

            if (percent >= 100)
            {
                percent = 0;
            }

            int currentPercent;
            if (percent < 50)
            {
                // On the first half of the animation the component is
                // on its initial size
                currentPercent = percent;
                isFaceDown = isFromFaceDownToFaceUp;
            }
            else
            {
                // On the second half of the animation the component
                // is flipped
                currentPercent = 100 - percent;
                isFaceDown = !isFromFaceDownToFaceUp;
            }
            // Shrink and widen the component to look like it is flipping
            destination =
                new Rectangle((int)(this.position.X +
                        this.sprite.Width * currentPercent / 100),
                    (int)this.position.Y,
                    (int)(this.sprite.Width - this.sprite.Width *
                        currentPercent / 100 * 2),
                    this.sprite.Height);
        }

        public void DrawSprite(SpriteBatch spriteBatch, Texture2D texture=null, float layerDepth=1f)
        {
            if (texture == null)
            {
                spriteBatch.Draw(sprite, position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, layerDepth);
                if (IsCovered)
                    spriteBatch.Draw(overlay, position - new Vector2(3), null, Color.White, 0f,
                        Vector2.Zero, 1f, SpriteEffects.None, layerDepth-0.05f);
            }
            else
            {
                if (isFaceDown)
                {
                    spriteBatch.Draw(sprite, destination, null, Color.White, 0f,
                        Vector2.Zero, SpriteEffects.None, layerDepth);
                }
                else
                {
                    spriteBatch.Draw(texture, destination, null, Color.White, 0f,
                        Vector2.Zero, SpriteEffects.None, layerDepth);
                }
            }
        }

        public CardSprite Clone(Vector2 pos)
        {
            return new CardSprite(this.sprite, overlay, pos, rank, cardSuit);
        }

        public CardSprite Clone()
        {
            return new CardSprite(this.sprite, overlay, this.position, rank, cardSuit);
        }

        public static bool operator ==(CardSprite card1, CardSprite card2)
        {
            if (card1.cardSuit == card2.cardSuit && card1.rank == card2.rank)
                return true;
            else
                return false;
        }

        public static bool operator !=(CardSprite card1, CardSprite card2)
        {
            return !(card1 == card2);
        }

        public override bool Equals(object obj)
        {
            if (obj is CardSprite)
            {
                CardSprite card = (CardSprite)obj;
                return card == this;
            }
            else
                throw new ArgumentException("obj is not type CardSprite.");
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

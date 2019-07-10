using System;
using System.Collections.Generic;

namespace StrangeSuits
{
    static class AIOpponent
    {
        public delegate int computerOpponents(AIHand hand, DiscardPile pile, Suit? suit);
        public static computerOpponents Opponent;
        public static void Init(MenuStage menuStage)
        {
            switch (menuStage)
            {
                case MenuStage.VeryEasy:
                    Opponent = new computerOpponents(computerVersion1); break;
                case MenuStage.Easy:
                    Opponent = new computerOpponents(computerVersion2); break;
                case MenuStage.Medium:
                    Opponent = new computerOpponents(computerVersion3); break;
                case MenuStage.Hard:
                case MenuStage.VeryHard:
                    Opponent = new computerOpponents(computerVersion4); break;
            }
        }
        private static int computerVersion1(AIHand playerCards2, DiscardPile discardPile, Suit? changedSuit)
        {
            int compIndex;
            for (compIndex = 0; compIndex < playerCards2.Count; compIndex++)
            {
                if (playerCards2[compIndex].CardSuit == changedSuit || playerCards2[compIndex].Rank == discardPile[discardPile.Count - 1].Rank
                    || playerCards2[compIndex].Rank == RankValue.Eight)
                    return compIndex;
            }
            return -1;
        }
        private static int computerVersion2(AIHand playerCards2, DiscardPile discardPile, Suit? changedSuit)
        {
            int card;
            for (card = 0; card < playerCards2.Count; card++)
            {
                if (changedSuit == playerCards2[card].CardSuit &&
                    (playerCards2[card].Rank == RankValue.Four || playerCards2[card].Rank == RankValue.Deuce
                        || playerCards2[card].Rank == RankValue.Three))
                    return card;
            }
            for (card = 0; card < playerCards2.Count; card++)
            {
                if (discardPile.Count > 1 && discardPile[discardPile.Count - 1] == discardPile[discardPile.Count - 2]
                    && playerCards2[card].Rank == discardPile[discardPile.Count - 1].Rank)
                    return card;
            }
            for (card = 0; card < playerCards2.Count; card++)
            {
                if (playerCards2[card].CardSuit == changedSuit)
                    return card;
            }
            for (card = 0; card < playerCards2.Count; card++)
            {
                if (playerCards2[card].Rank == RankValue.Eight)
                    return card;
            }
            for (card = 0; card < playerCards2.Count; card++)
            {
                if (playerCards2[card].Rank == discardPile[discardPile.Count - 1].Rank)
                    return card;
            }
            return -1;
        }
        private static int computerVersion3(AIHand playerCards2, DiscardPile discardPile, Suit? changedSuit)
        {
            int card = -1;

            if (discardPile.Count > 0 && discardPile[discardPile.Count - 1].Rank == RankValue.Deuce)
            {
                card = playerCards2.Find(c => c.Rank == RankValue.Deuce);
                if (card != -1)
                    return card;
            }

            card = playerCards2.Find(c => c.Rank == RankValue.Three && c.CardSuit == changedSuit);
            if (card != -1)
            {
                int count = playerCards2.FindCount(c => changedSuit == c.CardSuit);
                if (count > 2)
                    return card;

                count = playerCards2.FindCount(c => c.Rank == RankValue.Three);
                if (count > 1)
                {
                    int[] indexes = playerCards2.FindAllIndex(c => c.Rank == RankValue.Three && c.CardSuit != changedSuit);
                    foreach (int found in indexes)
                    {
                        count = playerCards2.FindCount(c => c.CardSuit == playerCards2[found].CardSuit);
                        if (count > 2)
                            return card;
                    }
                }
            }

            card = playerCards2.Find(c => c.Rank == RankValue.Four && c.CardSuit == changedSuit);
            if (card != -1)
            {
                int counts = playerCards2.FindCount(c => changedSuit == c.CardSuit && c.Rank != RankValue.Three);
                if (counts > 1 && discardPile.Count > 0 && discardPile[discardPile.Count - 1].Rank != RankValue.Three)
                    return card;

                counts = playerCards2.FindCount(c => c.Rank == RankValue.Four);
                if (counts > 1)
                {
                    int[] indexes = playerCards2.FindAllIndex(c => c.Rank == RankValue.Four && c.CardSuit != changedSuit);
                    foreach (int found in indexes)
                    {
                        counts = playerCards2.FindCount(c => c.CardSuit == playerCards2[found].CardSuit && c.Rank != RankValue.Three);
                        if (counts > 1)
                            return card;
                    }
                }
            }

            card = playerCards2.Find(c => c.Rank == RankValue.Deuce && c.CardSuit == changedSuit
                && discardPile.Count > 0 && discardPile[discardPile.Count - 1].Rank != RankValue.Three);
            if (card != -1)
                return card;

            card = checkHandHasThirdRank(playerCards2, discardPile, changedSuit);
            if (card != -1)
                return card;

            card = checkHandSameRank(playerCards2, discardPile, changedSuit);
            if (card != -1)
                return card;

            card = playerCards2.Find(c => c.CardSuit == changedSuit && c.Rank != RankValue.Three && c.Rank != RankValue.Four);
            if (card != -1)
                return card;

            card = checkHandDiscardRank(playerCards2, discardPile, changedSuit);
            if (card != -1)
                return card;

            card = playerCards2.Find(c => c.Rank == RankValue.Eight);
            if (card != -1)
                return card;

            return -1;
        }
        private static int computerVersion4(AIHand playerCards2, DiscardPile discardPile, Suit? changedSuit)
        {
            int count = 0;
            int index = 0;

            if (discardPile.Count > 0 && discardPile[discardPile.Count - 1] == SSEngine.DeckSprites["QueenSpades"])
            {
                index = playerCards2.Find(c => c == SSEngine.DeckSprites["DeuceSpades"]);
                if (index != -1)
                    return index;
            }

            if (discardPile.Count > 0 && discardPile[discardPile.Count - 1].Rank == RankValue.Deuce)
            {
                count = playerCards2.FindCount(c => c.Rank == RankValue.Deuce);
                if (count == 1)
                    return playerCards2.Find(c => c.Rank == RankValue.Deuce);
                else if (count > 1)
                {
                    List<byte> sortCount = playerCards2.GetSelectSuitCounts(c => c.Rank == RankValue.Deuce);
                    byte[] suitCount = new byte[sortCount.Count];
                    sortCount.CopyTo(suitCount);
                    sortCount.Sort();
                    int[] indexes = playerCards2.FindAllIndex(c => c.Rank == RankValue.Deuce);
                    for (int i = 0; i < count; i++)
                        if (suitCount[i] == sortCount[sortCount.Count - 1])
                            return indexes[i];
                }
            }

            index = playerCards2.Find(c => c.Rank == RankValue.Three && c.CardSuit == changedSuit);
            if (index != -1)
            {
                count = playerCards2.FindCount(c => (changedSuit == c.CardSuit
                    && c.Rank != RankValue.Four) || c.Rank == RankValue.Eight);
                if (count > 2)
                    return index;

                count = playerCards2.FindCount(c => c.Rank == RankValue.Three);
                if (count > 1)
                {
                    int[] indexes = playerCards2.FindAllIndex(c => c.Rank == RankValue.Three && c.CardSuit != changedSuit);
                    foreach (int found in indexes)
                    {
                        count = playerCards2.FindCount(c => c.CardSuit == playerCards2[found].CardSuit && c.Rank != RankValue.Four);
                        if (count > 2)
                            return index;
                    }
                }
            }

            index = playerCards2.Find(c => c.Rank == RankValue.Four && c.CardSuit == changedSuit);
            if (index != -1)
            {
                count = playerCards2.FindCount(c => changedSuit == c.CardSuit
                    && (c.Rank != RankValue.Three || c.Rank == RankValue.Eight));
                if (count > 1 && discardPile.Count > 0 && discardPile[discardPile.Count - 1].Rank != RankValue.Three)
                    return index;

                count = playerCards2.FindCount(c => c.Rank == RankValue.Four);
                if (count > 1)
                {
                    int[] indexes = playerCards2.FindAllIndex(c => c.Rank == RankValue.Four && c.CardSuit != changedSuit);
                    foreach (int found in indexes)
                    {
                        count = playerCards2.FindCount(c => c.CardSuit == playerCards2[found].CardSuit && c.Rank != RankValue.Three);
                        if (count > 1)
                            return index;
                    }
                }
            }

            if (discardPile.Count > 0 && discardPile[discardPile.Count - 1] != SSEngine.DeckSprites["ThreeSpades"]
                && changedSuit == Suit.Spades)
            {
                index = playerCards2.Find(c => c == SSEngine.DeckSprites["QueenSpades"]);
                if (index != -1)
                    return index;
            }

            if (discardPile.Count > 0 && discardPile[discardPile.Count - 1].Rank != RankValue.Three)
            {
                index = playerCards2.Find(c => c.Rank == RankValue.Deuce && c.CardSuit == changedSuit);
                if (index != -1)
                    return index;
            }

            index = checkHandHasThirdRank(playerCards2, discardPile, changedSuit);
            if (index != -1)
                return index;

            index = checkHandSameRank(playerCards2, discardPile, changedSuit);
            if (index != -1)
                return index;

            index = playerCards2.Find(c => c.Rank == RankValue.Ace && c.CardSuit == changedSuit);
            if (index != -1)
            {
                Suit suit = Suit.Club;
                switch (changedSuit)
                {
                    case Suit.Club:
                        suit = Suit.Spades; break;
                    case Suit.Spades:
                        suit = Suit.Club; break;
                    case Suit.Hearts:
                        suit = Suit.Diamond; break;
                    case Suit.Diamond:
                        suit = Suit.Hearts; break;
                }
                int suitCount = playerCards2.FindCount(c => c.CardSuit == changedSuit);
                int flippedSuitCount = playerCards2.FindCount(c => c.CardSuit == suit);
                if (flippedSuitCount > suitCount || suitCount == 1)
                    return index;
            }

            index = playerCards2.Find(c => c.CardSuit == changedSuit
                && c.Rank != RankValue.Ace && c.Rank != RankValue.Deuce && c.Rank != RankValue.Three && c.Rank != RankValue.Four
                && c.Rank != RankValue.Eight);
            if (index != -1)
                return index;

            if (discardPile.Count > 0)
            {
                count = playerCards2.FindCount(c => c.Rank == discardPile[discardPile.Count - 1].Rank);
                if (count > 1)
                {
                    List<byte> sortCount = playerCards2.GetSelectSuitCounts(c => c.Rank == discardPile[discardPile.Count - 1].Rank);
                    byte[] suitCount = new byte[sortCount.Count];
                    sortCount.CopyTo(suitCount);
                    sortCount.Sort();
                    int[] indexes = playerCards2.FindAllIndex(c => c.Rank == discardPile[discardPile.Count - 1].Rank);
                    for (int i = 0; i < count; i++)
                        if (suitCount[i] == sortCount[sortCount.Count - 1])
                            return indexes[i];
                }
            }

            index = playerCards2.Find(c => c.Rank == RankValue.Eight);
            if (index != -1)
                return index;

            index = checkHandDiscardRank(playerCards2, discardPile, changedSuit);
            if (index != -1)
                return index;

            return -1;
        }
        private static int computerVersion5(AIHand playerCards2, DiscardPile discardPile, Suit? changedSuit)
        {
            int count = 0;
            int index = 0;

            if (discardPile.Count > 0 && discardPile[discardPile.Count - 1] == SSEngine.DeckSprites["QueenSpades"])
            {
                index = playerCards2.Find(c => c == SSEngine.DeckSprites["DeuceSpades"]);
                if (index != -1)
                    return index;
            }

            if (discardPile.Count > 0 && discardPile[discardPile.Count - 1].Rank == RankValue.Deuce)
            {
                count = playerCards2.FindCount(c => c.Rank == RankValue.Deuce);
                if (count == 1)
                    return playerCards2.Find(c => c.Rank == RankValue.Deuce);
                else if (count > 1)
                {
                    List<byte> sortCount = playerCards2.GetSelectSuitCounts(c => c.Rank == RankValue.Deuce);
                    byte[] suitCount = new byte[sortCount.Count];
                    sortCount.CopyTo(suitCount);
                    sortCount.Sort();
                    int[] indexes = playerCards2.FindAllIndex(c => c.Rank == RankValue.Deuce);
                    for (int i = 0; i < count; i++)
                        if (suitCount[i] == sortCount[sortCount.Count - 1])
                            return indexes[i];
                }
            }

            index = playerCards2.Find(c => c.Rank == RankValue.Three && c.CardSuit == changedSuit);
            if (index != -1)
            {
                count = playerCards2.FindCount(c => (changedSuit == c.CardSuit
                    && c.Rank != RankValue.Four) || c.Rank == RankValue.Eight);
                if (count > 2)
                    return index;

                count = playerCards2.FindCount(c => c.Rank == RankValue.Three);
                if (count > 1)
                {
                    int[] indexes = playerCards2.FindAllIndex(c => c.Rank == RankValue.Three && c.CardSuit != changedSuit);
                    foreach (int found in indexes)
                    {
                        count = playerCards2.FindCount(c => c.CardSuit == playerCards2[found].CardSuit && c.Rank != RankValue.Four);
                        if (count > 2)
                            return index;
                    }
                }
            }

            index = playerCards2.Find(c => c.Rank == RankValue.Four && c.CardSuit == changedSuit);
            if (index != -1)
            {
                count = playerCards2.FindCount(c => changedSuit == c.CardSuit
                    && (c.Rank != RankValue.Three || c.Rank == RankValue.Eight));
                if (count > 1 && discardPile.Count > 0 && discardPile[discardPile.Count - 1].Rank != RankValue.Three)
                    return index;

                count = playerCards2.FindCount(c => c.Rank == RankValue.Four);
                if (count > 1)
                {
                    int[] indexes = playerCards2.FindAllIndex(c => c.Rank == RankValue.Four && c.CardSuit != changedSuit);
                    foreach (int found in indexes)
                    {
                        count = playerCards2.FindCount(c => c.CardSuit == playerCards2[found].CardSuit && c.Rank != RankValue.Three);
                        if (count > 1)
                            return index;
                    }
                }
            }

            if (discardPile.Count > 0 && discardPile[discardPile.Count - 1] != SSEngine.DeckSprites["ThreeSpades"]
                && changedSuit == Suit.Spades)
            {
                index = playerCards2.Find(c => c == SSEngine.DeckSprites["QueenSpades"]);
                if (index != -1)
                    return index;
            }

            if (discardPile.Count > 0 && discardPile[discardPile.Count - 1].Rank != RankValue.Three)
            {
                index = playerCards2.Find(c => c.Rank == RankValue.Deuce && c.CardSuit == changedSuit);
                if (index != -1)
                    return index;
            }

            index = checkHandHasThirdRank(playerCards2, discardPile, changedSuit);
            if (index != -1)
                return index;

            index = checkHandSameRank(playerCards2, discardPile, changedSuit);
            if (index != -1)
                return index;

            index = playerCards2.Find(c => c.Rank == RankValue.Ace && c.CardSuit == changedSuit);
            if (index != -1)
            {
                Suit suit = Suit.Club;
                switch (changedSuit)
                {
                    case Suit.Club:
                        suit = Suit.Spades; break;
                    case Suit.Spades:
                        suit = Suit.Club; break;
                    case Suit.Hearts:
                        suit = Suit.Diamond; break;
                    case Suit.Diamond:
                        suit = Suit.Hearts; break;
                }
                int suitCount = playerCards2.FindCount(c => c.CardSuit == changedSuit);
                int flippedSuitCount = playerCards2.FindCount(c => c.CardSuit == suit);
                if (flippedSuitCount > suitCount || suitCount == 1)
                    return index;
            }

            index = playerCards2.Find(c => c.CardSuit == changedSuit
                && c.Rank != RankValue.Ace && c.Rank != RankValue.Deuce && c.Rank != RankValue.Three && c.Rank != RankValue.Four
                && c.Rank != RankValue.Eight);
            if (index != -1)
                return index;

            if (discardPile.Count > 0)
            {
                count = playerCards2.FindCount(c => c.Rank == discardPile[discardPile.Count - 1].Rank);
                if (count > 1)
                {
                    List<byte> sortCount = playerCards2.GetSelectSuitCounts(c => c.Rank == discardPile[discardPile.Count - 1].Rank);
                    byte[] suitCount = new byte[sortCount.Count];
                    sortCount.CopyTo(suitCount);
                    sortCount.Sort();
                    int[] indexes = playerCards2.FindAllIndex(c => c.Rank == discardPile[discardPile.Count - 1].Rank);
                    for (int i = 0; i < count; i++)
                        if (suitCount[i] == sortCount[sortCount.Count - 1])
                            return indexes[i];
                }
            }

            index = playerCards2.Find(c => c.Rank == RankValue.Eight);
            if (index != -1)
                return index;

            index = checkHandDiscardRank(playerCards2, discardPile, changedSuit);
            if (index != -1)
                return index;

            return -1;
        }

        public static void EightAbilityVE(ref Suit? changedSuit)
        {
            Random random = new Random();
            changedSuit = (Suit)random.Next(4);
        }
        public static void EightAbilityEM(ref Suit? changedSuit, AIHand playerCards2)
        {
            List<byte> suitCountColl = playerCards2.GetSuitCounts();
            byte[] byteArray = new byte[4];
            suitCountColl.CopyTo(byteArray);
            suitCountColl.Sort();
            byte counter = 0;
            foreach (var value in byteArray)
            {
                counter++;
                if (value == suitCountColl[3])
                {
                    if (counter == 1)
                        changedSuit = Suit.Club;
                    else if (counter == 2)
                        changedSuit = Suit.Diamond;
                    else if (counter == 3)
                        changedSuit = Suit.Hearts;
                    else if (counter == 4)
                        changedSuit = Suit.Spades;
                }
            }
        }
        public static void EightAbilityHVH(ref Suit? changedSuit, AIHand playerCards2)
        {
            EightAbilityEM(ref changedSuit, playerCards2);
        }

        private static int checkHandHasThirdRank(AIHand playerCards2, DiscardPile discardPile, Suit? changedSuit)
        {
            if (discardPile.Count > 1 && discardPile[discardPile.Count - 1].Rank == discardPile[discardPile.Count - 2].Rank)
                return playerCards2.Find(c => c.Rank == discardPile[discardPile.Count - 1].Rank);
            else
                return -1;
        }

        private static int checkHandSameRank(AIHand playerCards2, DiscardPile discardPile, Suit? changedSuit)
        {
            int[] sameSuits = playerCards2.FindAllIndex(c => c.CardSuit == changedSuit);
            foreach (int cards in sameSuits)
            {
                int value = playerCards2.FindCount(c => c.Rank == playerCards2[cards].Rank);
                if (value > 1)
                    return cards;
            }
            return -1;
        }

        private static int checkHandDiscardRank(AIHand playerCards2, DiscardPile discardPile, Suit? changedSuit)
        {
            if (discardPile.Count > 0)
                return playerCards2.Find(c => c.Rank == discardPile[discardPile.Count - 1].Rank
                    && c.Rank != RankValue.Three && c.Rank != RankValue.Four);
            else
                return -1;
        }
    }
}

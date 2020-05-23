using System;
using System.Collections.Generic;
using System.Text;

namespace Saboteur.Model
{
    /// <summary>
    /// アクションカード抽象クラス
    /// </summary>
    internal abstract class ActionCard : Card
    {
        public ActionCard(int index) : base(index)
        {
        }
    }
}

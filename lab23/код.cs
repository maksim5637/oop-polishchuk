using System;

namespace Lab23
{
    // Вузькі інтерфейси (ISP)
    public interface IWeapon
    {
        void Attack();
    }

    public interface IHealable
    {
        void Heal();
    }

    public interface IDialogue
    {
        void Talk();
    }

    // Реалізації
    public class WeaponSystem : IWeapon
    {
        public void Attack() => Console.WriteLine("Hero attacks with weapon!");
    }

    public class MedicalKit : IHealable
    {
        public void Heal() => Console.WriteLine("Hero heals using medical kit!");
    }

    public class DialogueManager : IDialogue
    {
        public void Talk() => Console.WriteLine("Hero talks to NPC!");
    }

    // Головний клас (тепер залежить від абстракцій, а не конкретних класів)
    public class HeroAction
    {
        private readonly IWeapon weapon;       private readonly IHealable healer;
        private readonly IDialogue dialogue;

        // Dependency Injection через конструктор
        public HeroAction(IWeapon weapon, IHealable healer, IDialogue dialogue)
        {
            this.weapon = weapon;
            this.healer = healer;
            this.dialogue = dialogue;
        }

        public void Attack() => weapon.Attack();
        public void Heal() => healer.Heal();
        public void Talk() => dialogue.Talk();
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Конфігурація залежностей у Main
            IWeapon weapon = new WeaponSystem();
            IHealable healer = new MedicalKit();
            IDialogue dialogue = new DialogueManager();

            HeroAction hero = new HeroAction(weapon, healer, dialogue);

            hero.Attack();
            hero.Heal();
            hero.Talk();
        }
    }
}

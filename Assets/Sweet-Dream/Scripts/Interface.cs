public interface IInteractable
{
    void Interact();
}

public interface IDamageable
{
    void TakeDamage() { }
}
public interface IDamageDealer
{
    void DealDamage() {  }
}
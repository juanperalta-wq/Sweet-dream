// Tipos de comportamiento que puede tener una sombra. No son "tipos de enemigo" en el
// sentido de DataBaseEnemy/EnemyType (eso sigue siendo para los enemigos físicos como
// Barney); esto es exclusivamente la forma en la que una alucinación se comporta.
public enum ShadowType
{
    Watcher,   // Se queda inmóvil observando al jugador desde la distancia.
    Stalker,   // Persigue lentamente, manteniéndose detrás/en la periferia del jugador.
    Rusher,    // Corre directo hacia el jugador en cuanto aparece.
    Phantom    // Aparece, se muestra brevemente y se reubica en otro punto oculto (o desaparece), generando paranoia.
}
using AbcSynergy;

const int SQUAD_SIZE = 8;
const int RANDOM_SEED = 123;

var weakLinkFinder = new WeakLinkFinder();
weakLinkFinder.Execute(RANDOM_SEED, SQUAD_SIZE, 100);

// var oracle = new Oracle();
// oracle.Execute(RANDOM_SEED, SQUAD_SIZE);
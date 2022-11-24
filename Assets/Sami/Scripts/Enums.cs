public enum ObjectType { text, choice };    // tells what type of content the EventObject has
public enum Direction { left, right};       // used in moving character portraits in events
public enum Sect { Camarilla, Anarch};
public enum PredatorType { Siren, Sandman, Consensualist, Bagger, Alleycat};
public enum Clan { Brujah, Gangrel, Toreador, Tremere, Ventrue};
public enum Attribute { Social, Physical, Mental};
public enum Skill { Firearms, Stealth, Larceny, drive, Brawl, Athletics,
                    Investigation, Academics, Occult, Politics, Awareness,
                    Insight, Persuasion, Streetwise, Subterfuge, Science, Etiquette, Resource };
public enum CheckType { skill, boolean, sect, humanity, predatorType}
public enum EventLocation { haven, bar, cathedral, subway, chantry, clinic, mausoleum, representative, ball, factory, library, twp, none }
public enum RewardType { skill, experience, boolean, feeding, bloodBag, humanity, healthLoss, negativeBool}
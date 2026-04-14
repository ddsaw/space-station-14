# _N14/_MS13 Texture Attribution

All textures in this directory were sourced from the
[Nuclear-14 (Misfit Sanctuary)](https://github.com/Misfit-Sanctuary/nuclear-14)
SpaceStation 14 fork, which itself adapted sprites originally created for
[Mojave Sun 13](https://github.com/Mojave-Sun/mojave-sun-13) (SS13).

## License

**CC-BY-NC-SA-3.0** unless otherwise noted per-file.

Exceptions:
- `Tiles/concrete.png`, `Tiles/concretedark.png`, `Tiles/concreteroad.png` — **CC0-1.0**
  (original by Mithrandalf for N14, road modified by Peptide)
- `Structures/Furniture/bookshelf.rsi` — **CC0-1.0** (Mithrandalf for N14)
- `Structures/Furniture/Tables/schooldesk.rsi`, `Tables/desks.rsi` — **CC0-1.0** (Mithrandalf for N14)

## Source Breakdown

### Structures/Walls/tall/ & Structures/Walls/half/
- **Origin:** Mojave Sun 13
- **Ported to SS14 by:** Peptide90, Kill_Me_I_Noobs
- **License:** CC-BY-NC-SA-3.0
- Each RSI's `meta.json` contains the specific commit-level attribution.

### Structures/Walls/tallobstacles.rsi/
- **Origin:** Mojave Sun 13 (`mojave/icons/obstacles/tallobstacles.dmi`)
- **License:** CC-BY-NC-SA-3.0

### Structures/Decoration/barrels.rsi
- **Created by:** INFRARED_BARON for MS13
- **License:** CC-BY-NC-SA-3.0

### Structures/Decoration/barriers.rsi, cave_decor.rsi, world.rsi, signs_64x64.rsi
- **Origin:** Mojave Sun 13
- **License:** CC-BY-NC-SA-3.0

### Structures/Decoration/flora.rsi
- **Origin:** Nukapop-13
- **License:** CC-BY-NC-SA-3.0

### Structures/Decoration/torches.rsi
- **Origin:** Nukapop-13
- **License:** CC-BY-NC-SA-3.0

### Structures/Doors/
- **FalloutDoors/**: DesertRose (wooddoor, housedoor, metaldoor, celldoor, irondoor, etc.);
  Nukapop-13 (tentcloth, spikedgate); Fallout-13 (chaingate, modified by Peptide90)
- **TallDoors/**: MS13, ported by Peptide (wood, fence, metal variants);
  INFRARED_BARON for MS13 (vaultdoor — 128×64)
- **Curtains/**: Interstate-80, modified by Peptide90
- **RollerDoors/**: Created by Peptide90 for Nuclear14
- **SlantedDoors/**: MS13, ported by Peptide
- **License:** CC-BY-NC-SA-3.0

### Structures/Furniture/
- **chairs.rsi**: MS13 (`chairs.dmi`), modified to 32×32 by Peptide90
- **cooking.rsi**: Created by INFRARED_BARON for MS13
- **64x64_furniture.rsi**: MS13 (desks, wide tables, pool table, shelves)
- **64x96_furniture.rsi**: MS13, modified by Peptide90 (tall shelves, clothing rack)
- **bedsandchairs.rsi, junk.rsi**: Interstate-80, modified by Peptide90
- **bookshelf.rsi, schooldesk.rsi, desks.rsi**: CC0-1.0, Mithrandalf for N14
- **np13_misc.rsi**: Nukapop-13 (shelves, dresser, grandfather clock, fitness equipment)
- **Tables/barcounter.rsi, counters.rsi, metalgrate.rsi, table_settler.rsi, tables.rsi**: MS13
- **rugs.rsi, rugs64x64.rsi**: MS13
- **barricades.rsi, ms13_barricades.rsi**: MS13 / INFRARED_BARON
- **plantpots.rsi**: TGStation, modified by Peptide90
- **televisions.rsi**: MS13
- **Store_Furniture/**: MS13 (delistand, fruitstand)
- **Street_Furniture/**: MS13 (streetsigns); N14 originals by @Gary-McGaryson
  (streetlights, powerpole)
- **License:** CC-BY-NC-SA-3.0 (exceptions noted above)

### Tiles/
- See `Tiles/attributions.yml` for detailed per-file attribution.
- Major sources: Fortuna SS13 (Peptide90), Mojave Sun 13, Baystation12, Goonstation,
  Nuclear-14 originals (Mithrandalf, maxxorion, Zeta_Null).

## Usage in RedTruce

These textures are used under CC-BY-NC-SA-3.0 for a non-commercial SS14 fork.
Prototype definitions referencing these textures live in
`Resources/Prototypes/_RedTruce/`.

cmd-rpg-randomize-desc = Randomizes RedTruce RPG stat baselines (and skills) on your character (admin / debug).
cmd-rpg-randomize-help = Usage: rpg_randomize — requires Debug admin. Stats: only baseline is randomized; effective (current) is set equal to baseline (dice still use effective/current). Skills: category/spec baseline and current stay matched. Re-open the Character menu to refresh.
cmd-rpg-randomize-no-player = You must control a mob in-game to use this command.
cmd-rpg-randomize-missing = Your mob has no RTStats or RTSkills component (not a RedTruce RPG mob).
cmd-rpg-randomize-done = RPG stats and skills randomized. Open the Character menu again to refresh the sheet.
cmd-rpg-roll-skill-desc = Rolls a RedTruce skill effective dicepool against a TN (admin / debug).
cmd-rpg-roll-skill-help = Usage: rpg_roll_skill <categoryId> <tn> OR rpg_roll_skill <categoryId> <specId> <tn>. Use '-' or 'none' for no specialization.
cmd-rpg-roll-skill-no-player = You must control a mob in-game to use this command.
cmd-rpg-roll-skill-invalid-args = Invalid arguments. Use: rpg_roll_skill <categoryId> <tn> OR rpg_roll_skill <categoryId> <specId> <tn>.
cmd-rpg-roll-skill-invalid-tn = Invalid TN '{$tn}' (must be an integer).
cmd-rpg-roll-skill-invalid-category = Unknown skill category id '{$category}'.
cmd-rpg-roll-skill-invalid-spec = Unknown skill specialization id '{$spec}'.
cmd-rpg-roll-skill-missing = Your mob has no RTStats or RTSkills component (not a RedTruce RPG mob).
cmd-rpg-roll-skill-result = Rolled category={$category} spec={$spec} pool={$pool} tn={$tn} -> successes={$successes} | faces=[{$faces}]

rt-melee-roll-outcome-parried = PARRIED
rt-melee-roll-outcome-hit = HIT
rt-melee-roll-debug = [RT] melee {$attackerPool} vs {$defenderPool} @TN{$tn} | A {$attackerSuccesses} [{$attackerFaces}] / D {$defenderSuccesses} [{$defenderFaces}] => {$outcome}

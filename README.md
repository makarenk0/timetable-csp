# timetable-genetic-algorithm

### Forms a timetable for IT faculty using genetic algorithm

Conflicts which algorithm resolves:
- Same audiences between specialties (Computer science, Software engineering, Applied math) conflict
- Same teachers on different lessons at the same time conflict
- Non lecture audiences problem

If algorithm sees low/negative progress it will mutate special percentage of chromosomes (**`percentageOfMutations`** field in `GeneticMachine.cs`)

The input data and limitations are written in **`data.json`** file<br>
### Main rules:
- Each subject must have one lecturer and must be listed in `"Subject_lecturer"` property
- All teachers must be listed in `"Teacher"`
- All audiences must be listed in `"Audience"`
- All working days must be listed in `"WorkingDays"`
- All subjects must be listed in `"Subject"`
- All specialties must be listed in `"Specialty"`
- All lessons time range must be listed in `"Lessons_time"`
- All groups of special specialty must be listed in `*Specialty title here*_groups"`
- Other properties are limitaions on this data

`Program.cs` the entry point of a program consists of lines: <br>

 **`GeneticMachine gen = new GeneticMachine("../../../data.json", 15);`**<br>
 **`gen.FindAnswer();`**<br>
 
 You can pass in `GeneticMachine` path to data file and pass a number of starting population(number of chromosomes) <br>
 ### Example of one generation console output:
 

`Generation number = 0` <br>
`Chromosome 1 , Likelihood: 4,0300055 % , Fitness: 20 `<br>
`Chromosome 2 , Likelihood: 5,757151 % , Fitness: 14 `<br>
`Chromosome 3 , Likelihood: 5,037507 % , Fitness: 16 `<br>
`Chromosome 4 , Likelihood: 4,741183 % , Fitness: 17 `<br>
`Chromosome 5 , Likelihood: 6,200009 % , Fitness: 13 `<br>
`Chromosome 6 , Likelihood: 8,060011 % , Fitness: 10 `<br>
`Chromosome 7 , Likelihood: 7,3272834 % , Fitness: 11 `<br>
`Chromosome 8 , Likelihood: 6,200009 % , Fitness: 13 `<br>
`Chromosome 9 , Likelihood: 8,955568 % , Fitness: 9 `<br>
`Chromosome 10 , Likelihood: 6,716676 % , Fitness: 12 `<br>
`Chromosome 11 , Likelihood: 7,3272834 % , Fitness: 11 `<br>
`Chromosome 12 , Likelihood: 7,3272834 % , Fitness: 11 `<br>
`Chromosome 13 , Likelihood: 6,200009 % , Fitness: 13 `<br>
`Chromosome 14 , Likelihood: 8,060011 % , Fitness: 10 `<br>
`Chromosome 15 , Likelihood: 8,060011 % , Fitness: 10 `<br>
`Average fitness: 11,875`

***Example of full solution search in example1.txt***

ShapeTapper known flags (based on grepping PlayerPrefs.Get):

- bad
  - tracks all trials that have gone badly that have yet to be re-run
  - typically only has trials from one block
- badflag
  - whether we are running the bad trials at the moment or not
- block
  - current block number
- block_fb
  - whether or not we want feedback for a block
- block_percentage
  - passing percentage for a block
- configName
  - name of the configuration file
- Data
  - saved data for all runs
- endNum
  - last trial that was run (block relative)
- exit_flag
  - the exit status of the trial scene
    - 0 = end block
    - 1 = file read error
    - 9 = end of experiment
- feedbackLine
- lastBlockLine
- line
- numCorrect
- practice
- startNum
- userID
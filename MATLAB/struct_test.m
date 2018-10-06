level = 5;
semester = 'Fall';
subject = 'Math';
student = 'John_Doe';
fieldnames = {semester subject student};

% Add data to a structure named grades.
grades(level).(semester).(subject).(student)(10,21:30) = ...
             [85, 89, 76, 93, 85, 91, 68, 84, 95, 73];

% Retrieve the data added.
getfield(grades, {level}, fieldnames{:}, {10,21:30})
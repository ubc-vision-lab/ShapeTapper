UnityPort = tcpip('localhost', 56789, 'NetworkRole', 'Server');
set(UnityPort, 'InputBufferSize', 900000);

fprintf('waiting for client\n');
fopen(UnityPort);
pause(1);

fprintf('client connected\n');

if(UnityPort.BytesAvailable)
    greeting = fscanf(UnityPort);
    greeting = strtrim(greeting);
    disp(greeting);
else
    
end

run_experiment = true;
network_pause = 0.002;
current_state = 1; % state machine behaviour
% 1 = waiting for unity (finger on homebutton)
% 2 = eyetracking, waiting for optotrack signal
% 3 = eyetracking + optotrack, waiting for end/exit
% for 2 and 3, when eyetracking sees that eye out of range, it will
% stop recording for all devices and send the 'Restart' signal to Unity

while(run_experiment)
    % check is server connection still there
    switch current_state
        case 1 % waiting for Unity to signal start
            if(UnityPort.BytesAvailable)
                disp(['MATLAB State: ' int2str(current_state)]);
                data = fscanf(UnityPort);
                data = strtrim(data);
                if contains(data, 'Eyelink')
                    % fetch the trial number also?
                    current_state = 2;
                    WaitSecs(0.010);
                    fprintf(UnityPort,'Fixation'); % trick for getting unity to keep running
                    disp('Fixation sent');
%                     Eyelink('StartRecording'); % start recording
                elseif contains(data,'Exit') % quit, maybe done experiment
                    run_experiment = false;
                end
                WaitSecs(network_pause);
                disp(['Message Received: ' data ': ' int2str(current_state)]);
                data = '';
                
            end
        case 2 % eyetrack -- in a sense, MATLAB is in more control here
            if(true) % eye is okay
                % check for message to start recording optotrack
                if(get(UnityPort,'BytesAvailable')>0)
                    data = fscanf(UnityPort);
                    data = strtrim(data);
                    if contains(data,'Optotrack')
% -------------- Start the optotrack recording here! -------------------- %
                        %start collecting data, the number of frames to collect was
                        %pre-specified by the function OPTOTRAKSETUPCOLLECTION.
%                         DataBufferStart();
                        current_state = 3;
                    elseif contains(data,'Restart')
                        current_state = 1;
                    elseif contains(data,'End') %Unity says to kill?!
                        current_state = 1;
                    elseif contains(data,'Exit') % quit, for whatever reason
                        current_state = 1; % this doesn't matter cause it won't loop again
                        run_experiment = false;
                    end
                    WaitSecs(network_pause);
                    disp(['Message Received: ' data ': ' int2str(current_state)]);
                    data = '';
                end
            else % eye moved, let's start again
                fprintf(UnityPort,'Restart');
                disp("MATLAB sending restart");
                current_state = 1;
            end
        case 3 % eyetracking and optotrack both on
            if(UnityPort.BytesAvailable > 0)
                data = fgetl(UnityPort);
                data = strtrim(data);
                if contains(data,'End')
                    current_state = 4;
                elseif contains(data,'Exit') % quit, for whatever reason
                    current_state = 4;
                    run_experiment = false;
                elseif contains(data,'Restart') % bad trial
                    current_state = 4;
                end
                WaitSecs(network_pause);
                disp(['Message Received: ' data ': ' int2str(current_state)]);
                data = '';
            end
        case 4
            current_state = 1;
    end
end


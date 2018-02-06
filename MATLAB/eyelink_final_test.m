clc
clear all;
commandwindow;
 


import java.awt.Robot;
mouse = Robot;
sca

KbName('UnifyKeyNames');  
escKEY = KbName('escape');
spacekey=KbName('space');
pkey = KbName('p');
Screen('Preference', 'SkipSyncTests', 2)

num_trials = 4;        %change this if needed.          %change this if needed.                   %change this if needed.
x_threshold = 1200;    %change this if needed.          %change this if needed.                   %change this if needed.



prompt = {'Enter subject ID from shapetapper (eg: ab12)'};
dlg_title = 'Subject ID';
num_lines= 1;
def     = {'DEMO'};
answer  = inputdlg(prompt,dlg_title,num_lines,def);  




screenNumber=max(Screen('Screens'));
[window, wRect]=Screen('OpenWindow', screenNumber, 0,[],[],2);
Screen(window,'BlendFunction',GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
img = imread('cali_all.png');
texture1 = Screen('MakeTexture', window, img);

if EyelinkInit()~= 1; %
    return;
end;

el=EyelinkInitDefaults(window);

Eyelink('Command', 'link_sample_data = LEFT,RIGHT,GAZE,AREA');

[v vs]=Eyelink('GetTrackerVersion');
edfFile=strcat(answer{1},'.edf');
Eyelink('Openfile', edfFile);

eye_used = el.RIGHT_EYE;



while 1 % loop till error or space bar is pressed
        
        % check for keyboard press
        [keyIsDown,secs,keyCode] = KbCheck;
        % if spacebar was pressed stop display
        if keyCode(spacekey)
            KbReleaseWait;
            break;
        end
        if keyCode(escKEY)
            sca
            Eyelink('ShutDown');
            return;
        end
        
               
                    Screen('FillRect', window, el.backgroundcolour);
                    Screen('DrawTexture', window, texture1, [], [0 0 1920 1080]);
                    Screen('Flip',  el.window);
               
        
end % main loop

   

       





%HideCursor;
t = udp('localhost',64000,'LocalPort',64001, 'timeout', 1);
fopen(t);


allData = [];





sca



running = '0';
working = 1;
doing_trials = 1;

doing_block = 1;
doing_trial = 1;











while working
    doing_trial = 1;
    screenNumber=max(Screen('Screens'));
    [window, wRect]=Screen('OpenWindow', screenNumber, 0,[],32,2);
    Screen(window,'BlendFunction',GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
    img = imread('cali_all.png');
    texture1 = Screen('MakeTexture', window, img);
    
    
    EyelinkDoTrackerSetup(el);
    Eyelink('StartRecording');

    
    while 1 && working % loop till error or space bar is pressed
        % Check recording status, stop display if error
        error=Eyelink('CheckRecording');
        if(error~=0)
            break;
        end
        % check for keyboard press
        [keyIsDown,secs,keyCode] = KbCheck;
        % if spacebar was pressed stop display
        if keyCode(spacekey)
            break;
        end
        % check for presence of a new sample update
        if Eyelink( 'NewFloatSampleAvailable') > 0
            % get the sample in the form of an event structure
            evt = Eyelink( 'NewestFloatSample');
            
                % if we do, get current gaze position from sample
                x = evt.gx(eye_used+1); % +1 as we're accessing MATLAB array
                y = evt.gy(eye_used+1);
                % do we have valid data and is the pupil visible?
                if x~=el.MISSING_DATA & y~=el.MISSING_DATA & evt.pa(eye_used+1)>0
                    % if data is valid, draw a circle on the screen at current gaze position
                    % using PsychToolbox's Screen function
                    gazeRect=[ x-7 y-7 x+8 y+8];
                    
                    x_str = num2str(x);
                    y_str = num2str(y);
                    
                    mos_pos = strcat(x_str, ',', y_str);
                    
                    Screen('DrawTexture', window, texture1, [], [0 0 1920 1080]);
                    Screen('FrameOval', window, el.foregroundcolour,gazeRect,6,6);
                    
                    Screen('DrawText', window, mos_pos, x+50, y+50)
                    
                    Screen('Flip',  el.window, [], 1); % don't erase
                    x_threshold = x+200;
                else
                    % if data is invalid (e.g. during a blink), clear display
                    Screen('FillRect', window, el.backgroundcolour);
                    Screen('DrawTexture', window, texture1, [], [0 0 1920 1080]);
                    Screen('Flip',  el.window);
                end
        end % if sample available
    end % main loop
    
    
    Eyelink('StopRecording');
    sca
    
    
    
    while doing_trials && working
        loop_counter = 0;
              
        while running == '0' && working && doing_trials
            [keyIsDown,secs,keyCode] = KbCheck;
            % if spacebar was pressed stop display
            if keyCode(escKEY)
                working = 0;
                break
            end
            if keyCode(pkey)
                KbReleaseWait;
                doing_trials = 0;
            end
        
            running = fscanf(t);
            
            if isempty(running)
                running = '0';
            end
       
        end
   
        tic
        
        Eyelink('Message', 'SYNCTIME');
        Eyelink('StartRecording');
  
   
        while running ~= '0' && working && doing_trials% loop till error or space bar is pressed
            loop_counter = loop_counter + 1;
            running = fscanf(t);
            if isempty(running)
                running = '0';
            end
       
        
        
            % Check recording status, stop display if error
            error=Eyelink('CheckRecording');
            if(error~=0)
                break;
            end
            % check for keyboard press
        
            % check for presence of a new sample update
            if Eyelink( 'NewFloatSampleAvailable') > 0
                % get the sample in the form of an event structure
                evt = Eyelink( 'NewestFloatSample');
            
                % if we do, get current gaze position from sample
                x = evt.gx(eye_used+1); % +1 as we're accessing MATLAB array
                y = evt.gy(eye_used+1);
                % do we have valid data and is the pupil visible?
                if x~=el.MISSING_DATA & y~=el.MISSING_DATA & evt.pa(eye_used+1)>0
                    % if data is valid, draw a circle on the screen at current gaze position
                    % using PsychToolbox's Screen function
                    mouse.mouseMove(x, y);
                   
                    if x < x_threshold
                        fwrite(t, 49);
                    else
                        fwrite(t, 48);
                    end
                    
                    to_store = [doing_block, doing_trial, str2double(running), x, y, toc];
                    allData = [allData; to_store];

                else
                    fwrite(t, 48);
                end
            end % if sample available
        end % main loop
        
        
        Eyelink('StopRecording');
        doing_trial = doing_trial + 1;
        
        
    end %of trials
    

    doing_block = doing_block + 1;
    doing_trials = 1;

end


Eyelink('StopRecording');
    
 


WaitSecs(0.1);

    % STEP 7
    % finish up: stop recording eye-movements,
    % close graphics window, close data file and shut down tracker
   
    Eyelink('CloseFile');
    % download data file
    try
        fprintf('Receiving data file ''%s''\n', edfFile );
        status=Eyelink('ReceiveFile');
        if status > 0
            fprintf('ReceiveFile status %d\n', status);
        end
        if 2==exist(edfFile, 'file')
            fprintf('Data file ''%s'' can be found in ''%s''\n', edfFile, pwd );
        end
    catch
        fprintf('Problem receiving data file ''%s''\n', edfFile );
    end
    
    Eyelink('ShutDown');


    Screen('CloseAll');
    
    fclose(t);
delete(t);

filename = strcat(answer{1},'.txt');
csvwrite(filename, allData)



%ShowCursor
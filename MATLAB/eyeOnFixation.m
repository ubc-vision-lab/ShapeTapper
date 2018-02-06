function inRange = eyeOnFixation(fixation_pos, threshold, el, eye_used)
    if Eyelink('NewFloatSampleAvailable') > 0
        % get the sample in the form of an event structure
        evt = Eyelink( 'NewestFloatSample');

        % if we do, get current gaze position from sample
        x = evt.gx(eye_used+1); % +1 as we're accessing MATLAB array
        y = evt.gy(eye_used+1);

        % do we have valid data and is the pupil visible?
        if x~=el.MISSING_DATA && y~=el.MISSING_DATA && evt.pa(eye_used+1)>0
            % if the data is valid, check the position of it
            % relative to the position of the fixation on screen.
            % If it's bad, send a message back to Unity to abort,
            % and wait for the confirmation
            eye_pos = [x y];
            deviation_from_fixation = fixation_pos - eye_pos;
            if(norm(deviation_from_fixation) <= threshold)
                inRange = 1;
                break;
            else
                inRange = 0;
            end;
        else
            % data is missing... abort?
            inRange = 0;
        end
    end
end
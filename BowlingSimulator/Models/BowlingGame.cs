public class BowlingGame
{
    public int?[] Rolls { get; private set; } // Holds the pins knocked down for each roll
    public int?[] FrameScores { get; private set; } // Holds the indvidual score for each frame
    public int?[] RunningScores { get; private set; } // Holds the accumulative score for each frame

    public int CurrentFrame { get; private set; } // Tracks the current frame (zero-indexed)
    public int PendingRoll { get; private set; } // Tracks the next roll to happen (zero-indexed).  So 0 means we're on the first roll that hasn't happened yet
    public bool IsGameOver { get; private set; } // Is the game over?


    public BowlingGame()
    {
        Rolls = new int?[22]; // Max possible rolls in a game (10 frames plus bonus frame)
        FrameScores = new int?[10]; // Store score per frame
        RunningScores = new int?[10];  // accumulative score per frame
        PendingRoll = 0;
        CurrentFrame = 0;
    }

    public void Roll( int pins )
    {
        if( IsGameOver ) return;

        Rolls[PendingRoll] = pins; // record the number of pins knocked down

        // if this is not the bonus round
        if( PendingRoll < 20 ) {
            // if this is the first roll in the frame and 10 pins were knocked down
            if( pins == 10 && PendingRoll % 2 == 0 ) // Strike
            {
                PendingRoll++; // skip the second roll of the frame and move to the first roll of the next frame
                CurrentFrame++; // move to the next frame
            }
            else if( PendingRoll % 2 != 0 ) // If it's the second roll
            {
                CurrentFrame++;  // move to the next frame
            }
        }

        PendingRoll++;  // move to the next roll

        UpdateFrameScores();
    }


    public void UpdateFrameScores()
    {
        // PendingRoll (zero-based) = the next roll to happen (0 means we're on the first roll that hasn't happened yet)

        // loop through the frames and update FrameScores
        for( int frame = 0; frame < 10; frame++ ) {
            int rollIndex = frame * 2;  // roll index is always the first roll in the frame

            // if all 10 pins were knocked down in the first roll
            if( Rolls[rollIndex] == 10 ) // Strike
            {
                // if two more rolls after this frame have occurred
                if( PendingRoll > rollIndex + 3 )
                {
                    // the next two rolls are added to the points you get for a strike (rollIndex + 2 instead of 1 because second frame roll is skipped)
                    FrameScores[frame] = 10 + Rolls[rollIndex + 2];
                    // if the next roll was a strike as well
                    if( Rolls[rollIndex + 2] == 10 ) {
                        // if this is not the last frame
                        if( rollIndex < 18 ) {
                            FrameScores[frame] += Rolls[rollIndex + 4];  // then go with the first roll of the next frame
                        }
                        else {
                            // this is the last frame.  Go with the second roll in the bonus frame
                            FrameScores[frame] += Rolls[rollIndex + 3];
                        }
                    }
                    else FrameScores[frame] += Rolls[rollIndex + 3];  // otherwise go with the second roll
                }
                else {
                    FrameScores[frame] = null; // Score not yet determined
                }
            }
            // else if the sum of the first and second roll in the frame is 10
            else if( Rolls[rollIndex] + Rolls[rollIndex + 1] == 10 ) // Spare
            {
                // if an additional roll after this frame has occured
                if( PendingRoll > rollIndex + 2 )
                {
                    // the frame score gets the value of the next roll added to it
                    FrameScores[frame] = 10 + Rolls[rollIndex + 2];
                }
                else {
                    FrameScores[frame] = null; // Score not yet determined
                }
            }
            else // Regular frame
            {
                // if both rolls in frame have occured
                if( PendingRoll > rollIndex + 1 ) {
                    // add the two rolls in the frame
                    FrameScores[frame] = Rolls[rollIndex] + Rolls[rollIndex + 1];
                }
                else {
                    FrameScores[frame] = null; // Score not yet determined
                }
            }
        }

        for( int frame = 0; frame < 10; frame++ ) {
            RunningScores[frame] = frame == 0 ? FrameScores[frame] : RunningScores[frame-1] + FrameScores[frame];
        }

        // the game is over if the last frame score was determined
        IsGameOver = FrameScores[9] != null;
    }


    public int GetScore()
    {
        int score = 0;
        foreach( var frameScore in FrameScores ) {
            score += frameScore ?? 0;
        }

        return score;
    }

}

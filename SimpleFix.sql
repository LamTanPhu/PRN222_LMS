-- Simple fix for QuizQuestion QuestionType constraint
USE CourseraStyleLMS;

-- Drop the problematic constraint
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS 
    WHERE CONSTRAINT_NAME = 'CK_QuizQuest_Quest_04E4BC85'
)
BEGIN
    ALTER TABLE QuizQuestions DROP CONSTRAINT CK_QuizQuest_Quest_04E4BC85;
    PRINT 'Dropped constraint CK_QuizQuest_Quest_04E4BC85';
END

-- Add new constraint with correct values
ALTER TABLE QuizQuestions 
ADD CONSTRAINT CK_QuizQuestions_QuestionType 
CHECK (QuestionType IN ('Multiple Choice', 'True/False', 'Short Answer'));

PRINT 'Added new constraint successfully!';

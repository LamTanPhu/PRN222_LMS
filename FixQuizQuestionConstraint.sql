-- Fix QuizQuestion QuestionType CHECK Constraint
-- This script will fix the database constraint issue

USE CourseraStyleLMS;
GO

-- 1. Check existing constraints
SELECT 
    CONSTRAINT_NAME,
    CHECK_CLAUSE
FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS 
WHERE CONSTRAINT_NAME LIKE '%QuizQuest%QuestionType%';

-- 2. Drop the problematic constraint if it exists
IF EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS 
    WHERE CONSTRAINT_NAME = 'CK_QuizQuest_Quest_04E4BC85'
)
BEGIN
    ALTER TABLE QuizQuestions DROP CONSTRAINT CK_QuizQuest_Quest_04E4BC85;
    PRINT 'Dropped existing constraint CK_QuizQuest_Quest_04E4BC85';
END

-- 3. Add new constraint with correct values
ALTER TABLE QuizQuestions 
ADD CONSTRAINT CK_QuizQuestions_QuestionType 
CHECK (QuestionType IN ('Multiple Choice', 'True/False', 'Short Answer'));

PRINT 'Added new constraint CK_QuizQuestions_QuestionType';

-- 4. Verify the constraint
SELECT 
    CONSTRAINT_NAME,
    CHECK_CLAUSE
FROM INFORMATION_SCHEMA.CHECK_CONSTRAINTS 
WHERE CONSTRAINT_NAME = 'CK_QuizQuestions_QuestionType';

-- 5. Test with valid values
INSERT INTO QuizQuestions (QuizId, QuestionText, QuestionType, Points, SortOrder)
VALUES (1, 'Test Question', 'Multiple Choice', 1, 1);

-- Clean up test data
DELETE FROM QuizQuestions WHERE QuestionText = 'Test Question';

PRINT 'Constraint test completed successfully!';
GO

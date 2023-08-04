using Application.Commons;
using Application.Interfaces;
using Application.Repositories;
using Application.ViewModels.QuizModels;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IClaimsService _claimsservice;
        private readonly AppConfiguration _configuration;


        public QuestionService(IClaimsService claimsservice, IUnitOfWork unitOfWork, IMapper mapper, AppConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _claimsservice = claimsservice;
        }


        public async Task<List<CreateQuizIntoBankDTO>> Filter(ICollection<Guid> TopicList, ICollection<int> QuizType)
        {
            List<Question> quizBanks1 = await _unitOfWork.QuestionRepository.GetAllAsync();
            List<Question> quizBanks = new List<Question>();
            List<Question> quizType = new List<Question>();
            //throwlis new NotImplementedException();
            //var topicList = await _unitOfWork.QuizBankRepository.FilterQuizTestWithTopic(TopicList);
            foreach (var quizTest in QuizType)
            {
                Question comsuon = quizBanks1.Find(x => x.QuizTypeID == quizTest && x.IsDeleted == false);
                quizType.Add(comsuon);
            }
            foreach (var quizTest in TopicList)
            {
                Question comsuon = quizBanks1.Find(x => x.TopicID == quizTest && x.IsDeleted == false);
                quizBanks.Add(comsuon);
            }
            //var quizType = await _unitOfWork.QuizBankRepository.FilterQuizTestWithType(QuizType);
            List<Question> lastchane = new List<Question>();
            List<CreateQuizIntoBankDTO> createQuizIntoBankDTOs = new List<CreateQuizIntoBankDTO>();
            //CreateQuizIntoBankDTO QuestionMapper = _mapper.Map<CreateQuizIntoBankDTO>(lastchane);

            foreach (var topic in quizBanks)
            {
                foreach (var quiz in quizType)
                {
                    if (topic == quiz)
                    {
                        CreateQuizIntoBankDTO QuestionMapper = _mapper.Map<CreateQuizIntoBankDTO>(topic);
                        //lastchane.Add(topic);
                        createQuizIntoBankDTOs.Add(QuestionMapper);
                    }
                }

            }

            return createQuizIntoBankDTOs;
        }

        public async Task<List<CreateQuizIntoBankDTO>> Search(string SearchName)
        {
            //throw new NotImplementedException();
            var searchByName = await _unitOfWork.QuestionRepository.FindAsync(x => x.Content.Contains(SearchName) && x.IsDeleted == false);
            //var mapQuiz = _mapper.Map<CreateQuizIntoBankDTO>(searchByName);
            List<CreateQuizIntoBankDTO> comsuon = new List<CreateQuizIntoBankDTO>();
            foreach (var topic in searchByName)
            {
                var mapQuiz = _mapper.Map<CreateQuizIntoBankDTO>(topic);
                comsuon.Add(mapQuiz);
            }
            if (searchByName is null)
            {
                throw new Exception();

            }
            return comsuon;

        }

        public async Task<bool> AddQuestionToBank(CreateQuizIntoBankDTO quizDto)
        {
            // check correst answer exist in 4 answer
            if (quizDto.CorrectAnswer != quizDto.Answer1 &&
                quizDto.CorrectAnswer != quizDto.Answer2 &&
                quizDto.CorrectAnswer != quizDto.Answer3 &&
                quizDto.CorrectAnswer != quizDto.Answer4)
            {
                return false;
            }
            // check topic exist if null retrun false
            var checkTopicExist = await _unitOfWork.TopicRepository.GetByIdAsync(quizDto.TopicID);
            if (checkTopicExist == null)
            {
                return false;
            }
            // Create new question, dont check same content
            var newQuestion = new Question
            {
                Content = quizDto.Content,
                Answer1 = quizDto.Answer1,
                Answer2 = quizDto.Answer2,
                Answer3 = quizDto.Answer3,
                Answer4 = quizDto.Answer4,
                CorrectAnswer = quizDto.CorrectAnswer,
                TopicID = quizDto.TopicID,
                QuizTypeID = quizDto.TypeID,
                //CreationDate = DateTime.Now.Date,
                //CreatedBy = _claimsservice.GetCurrentUserId
            };
            await _unitOfWork.QuestionRepository.AddAsync(newQuestion);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> CreateEmptyQuizTest(CreateEmptyQuizDTO quizDto)
        {

            var newQuiz = new Quiz
            {
                NumberOfQuiz = 0,
                LectureID = quizDto.LectureID,
            };
            await _unitOfWork.QuizRepository.AddAsync(newQuiz);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> AddQuestionToQuizTest(AddQuestionToQuizTestDTO quizDto)
        {
            var newQuestionToTest = new DetailQuizQuestion
            {
                QuizID = quizDto.QuizId,
                QuestionID = quizDto.QuestionId,
                //SubmitQuizID = Guid.NewGuid(),  // tam thoi fix cung, quizDto.SubmitQuiz, 
            };
            // count +1 numberOfQuiz when 1 question add to quiz
            var getQuizTest = await _unitOfWork.QuizRepository.GetByIdAsync(quizDto.QuizId);
          
            getQuizTest.NumberOfQuiz += 1;
            _unitOfWork.QuizRepository.Update(getQuizTest);

            await _unitOfWork.DetailQuizQuestionRepository.AddAsync(newQuestionToTest);
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> UpdateQuizTest(Guid quizTestId, UpdateQuizTestDTO quizDto)
        {
            // check quiz test exist
            var checkQuizTestExist = await _unitOfWork.QuizRepository.GetByIdAsync(quizTestId);
            var checkCurrentQuestion = await _unitOfWork.QuestionRepository.GetByIdAsync(quizDto.IdQuestionWantToUpdate);
            var checkNewQuestion = await _unitOfWork.QuestionRepository.GetByIdAsync(quizDto.NewQuestionId);
            if (checkQuizTestExist == null || checkCurrentQuestion == null || checkNewQuestion == null)
            {
                return false;
            }
            // get all question of quiz test
            var allQuestionOfQuiz = await _unitOfWork.QuizRepository.GetAllQuestionByQuizTestId(quizTestId);
            bool check = false;
            foreach (var item in allQuestionOfQuiz)
            {
                //update
                if (item.QuestionID == quizDto.IdQuestionWantToUpdate)
                {
                    item.QuestionID = quizDto.NewQuestionId;
                    _unitOfWork.DetailQuizQuestionRepository.Update(item);
                    check = true;
                    break;
                }
            }
            if (!check) return false;
            return await _unitOfWork.SaveChangeAsync() > 0;
        }

        public async Task<bool> DeleteQuizTest(Guid quizTestId)
        {
            // check quiz test exist
            var checkQuizTestExist = await _unitOfWork.QuizRepository.GetByIdAsync(quizTestId);
            if (checkQuizTestExist == null)
            {
                return false;
            }
            _unitOfWork.QuizRepository.SoftRemove(checkQuizTestExist);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }

        public async Task<DoingQuizDTO> ViewDoingQuiz(Guid QuizID)
        {

            //throw new NotImplementedException();
            var listQuestionForTrainer = _unitOfWork.QuestionRepository.GetQuizForTrainer(QuizID);
            var listQuestionForTrainee = _unitOfWork.QuestionRepository.GetQuizForTrainee(QuizID);
            var number = await _unitOfWork.QuizRepository.GetByIdAsync(QuizID);
            DoingQuizDTO lastquiz = new DoingQuizDTO();

            var user = await _unitOfWork.UserRepository.GetByIdAsync(_claimsservice.GetCurrentUserId);
            if(user == null) {
                throw new Exception("Not found user");
            }
            if (user.RoleId == 4)
            {
                DoingQuizDTO doingQuizDTOTrainee = new DoingQuizDTO()
                {
                    NumberOfQuestion = number.NumberOfQuiz,
                    AnswerQuizQuestionDTOs = listQuestionForTrainee
                };
                lastquiz = doingQuizDTOTrainee;
            }
            else
            {
             
                DoingQuizDTO doingQuizDTOTrainer = new DoingQuizDTO()
                {

                    NumberOfQuestion = number.NumberOfQuiz,
                    QuizQuestionDTOs = listQuestionForTrainer,
                };
                lastquiz = doingQuizDTOTrainer;
            }

            return lastquiz;

        }


        public async Task<bool> DoingQuizService(ICollection<AnswerQuizQuestionDTO> answerQuizQuestionDTO)
        {
            /*  Những thứ cần lấy : AnswerSubmit ,IsCorect ,DetailQuizID,UserID , IsDeleted 
              AnswerSubmit = answerQuizQuestionDTO.id 
              IsCorrect when the  answersubmit ==  correct by QuestionID
              DetailQuizID lấy bằng  
              đăng nhập bằng ID của người dùng 
              IsDeleted bằng k
             để xác định answer có đúng hay không ta phải dựa vào câu trả lời của 
             */

            foreach (AnswerQuizQuestionDTO answer in answerQuizQuestionDTO)
            {
                var detailQuizQuestion = await _unitOfWork.DetailQuizQuestionRepository.GetByIdAsync(answer.DetailQuizQuestionID);

                if (detailQuizQuestion is not null)
                {
                    var question = await _unitOfWork.QuestionRepository.GetByIdAsync(detailQuizQuestion.QuestionID);
                    bool check = (question is not null && question.CorrectAnswer == answer.UserAnswer);
                    var submitFromUser = new SubmitQuiz()
                    {
                        CreationDate = DateTime.Now,
                        Id = new Guid(),
                        AnswerSubmit = answer.UserAnswer,
                        IsCorrect = check,
                        DetailQuizQuestionID = answer.DetailQuizQuestionID,
                        UserID = _claimsservice.GetCurrentUserId,
                        IsDeleted = false
                    };
                    var checkDuplicate = await _unitOfWork.SubmitQuizRepository.FindAsync(x => x.UserID == submitFromUser.UserID && x.DetailQuizQuestionID == submitFromUser.DetailQuizQuestionID);

                    if (checkDuplicate.Count == 0)
                    {
                        await _unitOfWork.SubmitQuizRepository.AddAsync(submitFromUser);
                        await _unitOfWork.SaveChangeAsync();
                    }
                }
            }
            return true;
        }



        public async Task<List<ViewDetailResultDTO>> ViewMarkDetail(Guid SubmitQuizID)
        {
            return _unitOfWork.SubmitQuizRepository.GetViewDetail(SubmitQuizID);
        }
        public async Task<bool> ViewGrading(Guid user_id, Guid class_id)
        {
            Guid GradingID = _unitOfWork.DetailTrainingProgramSyllabusRepository.TakeDetailTrainingID(user_id, class_id);
            await _unitOfWork.SaveChangeAsync();
            if (GradingID == Guid.Empty)
            {
                return false;
            }
            return true;

        }

        public async Task<double> MarkQuiz(Guid QuizID, Guid DetailTrainingDetailTrainingClassParticipateId)
        {
            User? user = await _unitOfWork.UserRepository.GetByIdAsync(_claimsservice.GetCurrentUserId);
            if (user == null)
            {
                throw new Exception("Not Found the User,please check again");
            }

          var check =  await _unitOfWork.DetailTrainingClassParticipateRepository.FindAsync(x => x.UserId == user.Id && x.Id == DetailTrainingDetailTrainingClassParticipateId);
            if(check.Count == 0)
            {
                throw new Exception("USer not join in this class");
            }
            double correct_answer = _unitOfWork.SubmitQuizRepository.CheckTrueAnswer(user.Id, QuizID);
            var quiz = await _unitOfWork.QuizRepository.GetByIdAsync(QuizID);
            if (quiz is null) {
                throw new Exception("Cannot Get The Quiz");
            }
           double number_of_question = quiz.NumberOfQuiz;
            double cont = 0; 
                cont = (correct_answer /
                number_of_question);
            double checkNum = cont * 10;
            int result = (int)checkNum;
            //string check11 = "" + checkNum;

            //int result = int.Parse(check11.Substring(0));
            //if (check11.Length > 1)
            //{
            //    if (int.Parse(check11.Substring(1)) == 0)
            //    {
            //        result = int.Parse(check11.Substring(0, 1));
            //    }
            //}
            await AddGrading(quiz.LectureID, DetailTrainingDetailTrainingClassParticipateId, result);
            return checkNum;
        }


        public async Task<bool> AddGrading(Guid LectureID, Guid DetailTrainingClassParticipateId, double TotalMark)
        {
            Grading new_grading = new Grading()
            {

                LectureId = LectureID,
                DetailTrainingClassParticipateId = DetailTrainingClassParticipateId,
                NumericGrade = TotalMark
            };

            await _unitOfWork.GradingRepository.AddAsync(new_grading);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }

        public async Task<AnswerQuizDetailTraineeDTO> ViewDetaildoneQuiz(Guid QuizID)
        {

            AnswerQuizDetailTraineeDTO object_answer = new AnswerQuizDetailTraineeDTO();
            var User = await _unitOfWork.UserRepository.GetByIdAsync(_claimsservice.GetCurrentUserId);
            var Quiz = await _unitOfWork.QuizRepository.GetByIdAsync(QuizID);
            if (Quiz is null) {
                throw new NullReferenceException("You Haven't Do the Test before");
            }
            object_answer.QuizName = Quiz.QuizName;
            object_answer.NumberOfQuiz = Quiz.NumberOfQuiz;
            object_answer.DoneQuiz = _unitOfWork.QuizRepository.ViewAnswer(QuizID, User.Id);

            return object_answer;
        }

        public async Task<bool> RemoveQuestionInBank(Guid QuestionId)
        {
            var FindQuestion = await _unitOfWork.QuestionRepository.GetByIdAsync(QuestionId);
            if (FindQuestion is null)
            {
                return false;
            }

            _unitOfWork.QuestionRepository.SoftRemove(FindQuestion);
            await _unitOfWork.SaveChangeAsync();
            return true;
        }

        public async Task<bool> UpdateQuestion(Guid QuestionID, UpdateQuestionDTO createQuizIntoBankDTO)
        {
            var QuestionFind = await _unitOfWork.QuestionRepository.GetByIdAsync(QuestionID);
            if(QuestionFind is null)
            {
                    return false;
            }
            QuestionFind.Answer4 = createQuizIntoBankDTO.Answer4;
            QuestionFind.Answer1 = createQuizIntoBankDTO.Answer1;
            QuestionFind.Answer2 = createQuizIntoBankDTO.Answer2;
            QuestionFind.Answer3 = createQuizIntoBankDTO.Answer3;
            QuestionFind.Content = createQuizIntoBankDTO.Content;
            QuestionFind.TopicID = createQuizIntoBankDTO.TopicID;
            QuestionFind.QuizTypeID = createQuizIntoBankDTO.TypeID;
            QuestionFind.CorrectAnswer = createQuizIntoBankDTO.CorrectAnswer;
            QuestionFind.IsDeleted = true;
            _unitOfWork.QuestionRepository.Update(QuestionFind);
            await _unitOfWork.SaveChangeAsync();
            return true;



        }


    }
}

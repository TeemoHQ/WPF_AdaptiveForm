using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaitingRoomBigScreen.WebService
{
    public class WebService
    {
        private readonly  DataHandler _dataHandler;

        public WebService(DataHandler dataHandler)
        {
            _dataHandler = dataHandler;
        }
        public  async Task<Result<List<QueueInfo>>> GetQueue(ReqQueue req)
        {
            var result = await _dataHandler.Query<ResQueue, ReqQueue>(req);
            if (!result.IsSuccess)
                return Result<List<QueueInfo>>.Convert(result);

            var res = result.Value;
            if (!res.success)
                return Result<List<QueueInfo>>.Fail(res.msg);
            return Result<List<QueueInfo>>.Success(res.data);
        }
        public async Task<Result<bool>> GetInitDevie(ReqInitDevie req)
        {
            var result = await _dataHandler.Query<ResInitDevie, ReqInitDevie>(req);
            if (!result.IsSuccess)
                return Result<bool>.Convert(result);

            var res = result.Value;
            if (!res.success)
                return Result<bool>.Fail(res.msg);
            return Result<bool>.Success(res.data);
        }
        public async Task<Result<string>> GetGetSecret(ReqGetSecret req)
        {
            var result = await _dataHandler.Query<ResGetSecret, ReqGetSecret>(req);
            if (!result.IsSuccess)
                return Result<string>.Convert(result);

            var res = result.Value;
            if (!res.success)
                return Result<string>.Fail(res.msg);
            return Result<string>.Success(res.data);
        }

        public  async Task<Result<string>> GetToken(ReqToken req)
        {
            var result = await _dataHandler.Query<ResToken, ReqToken>(req);
            if (!result.IsSuccess)
                return Result<string>.Convert(result);

            var res = result.Value;
            if (!res.success)
                return Result<string>.Fail(res.msg);
            return Result<string>.Success(res.data);
        }
    }
}

local task = {};

---marks that function in param can use await
---@param callback function function that can use await
---@return function function with coroutine
function task.async(callback)
    return function(...)
        local co = coroutine.create(function(...)
            local success, result = pcall(callback, ...)
            if not success then
                error(result)
            end
            return result
        end)

        local success, result = coroutine.resume(co, ...)
        if not success then
            error(result)  
        end
        return result
    end
end

---await dotnet task
---@param dotnet_task table System.Threading.Task task
---@return any task result
function task.await(dotnet_task)
    local co = coroutine.running()
    if not co then
        error("task.await can only be used within an async function")
    end

    dotnet_task:ContinueWith(function(completedTask)
        local success = completedTask.Status == 5  -- RanToCompletion
        if success then
            local result = completedTask
            if completedTask.GetType().GetProperty("Result") ~= nil then
                result = completedTask.Result
            end
            coroutine.resume(co, true, result)
        else
            local exception = completedTask.Exception
            if exception ~= nil then
                coroutine.resume(co, false, exception:ToString())
            else
                coroutine.resume(co, false, "Task failed without exception")
            end
        end
    end)

    local success, result = coroutine.yield()
    if not success then
        error(result) 
    end
    return result
end

return task
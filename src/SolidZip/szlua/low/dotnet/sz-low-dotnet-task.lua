local task = {};

---marks that function in param can use await
---@param callback function function that can use await
---@return function function with coroutine
function task.async(callback)
    return function(...)
        local co = coroutine.create(callback)
        coroutine.resume(co, ...)
    end
end

---await dotnet task
---@param dotnet_task table System.Threading.Task task
---@return any coroutine result
function task.await(dotnet_task)
    local co = coroutine.running()

    dotnet_task:ContinueWith(function(completedTask)
        coroutine.resume(co, completedTask)
    end)

    return coroutine.yield()
end

return task;
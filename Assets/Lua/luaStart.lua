print("Hello Lua!")

local test = {}
function test:add(a,b)
    print("test:add",a+b)
end

test:add(1,2);

for i = 1, 10000 do
    print("test gc "..i)
end

return test
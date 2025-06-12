# Tuple Return Type Pattern

## What It Is

Instead of throwing exceptions for validation errors, I return tuples with success/failure information:

```csharp
Task<(bool Success, List<string> Errors)>
Task<(bool Success, string Message)>
```

## Why I Use It

**Before (with exceptions):**
```csharp
public async Task AddDishAsync(Dish dish)
{
    if (string.IsNullOrEmpty(dish.Name))
        throw new ValidationException("Dish name is required");
}
```

**Now (with tuples):**
```csharp
public async Task<(bool Success, List<string> Errors)> AddDishAsync(Dish dish)
{
    var validation = await _validator.ValidateAsync(dish);
    if (!validation.IsValid)
        return (false, validation.Errors);
    
    // Continue with operation...
    return (true, new List<string>());
}
```

## Benefits

- No exceptions for expected failures like validation
- Multiple error messages at once
- Clear success/failure handling

## How I Use It

**In Validators:**
```csharp
public async Task<(bool IsValid, List<string> Errors)> ValidateAsync(Dish dish)
{
    var errors = new List<string>();
    
    if (string.IsNullOrEmpty(dish.Name))
        errors.Add("Name is required");
    
    return (errors.Count == 0, errors);
}
```

**In Managers:**
```csharp
public async Task<(bool Success, List<string> Errors)> AddDishAsync(Dish dish)
{
    var validation = await _validator.ValidateAsync(dish);
    if (!validation.IsValid)
        return (false, validation.Errors);
    
    await _repository.AddAsync(dish);
    return (true, new List<string>());
}
```

**In Pages:**
```csharp
var result = await _manager.AddDishAsync(dish);
if (result.Success)
{
    // Success - redirect or show success message
}
else
{
    // Show errors to user
    foreach (var error in result.Errors)
        ModelState.AddModelError("", error);
}
```

## When to Use

✅ **Use tuples for:**
- Validation failures
- Business rule violations
- Expected failures

❌ **Use exceptions for:**
- Database connection issues
- Programming errors
- Unexpected system failures


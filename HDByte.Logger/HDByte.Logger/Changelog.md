1.4.0 -> 2.0.0

Added LoggerManager unit tests.
Add RemoveLogger(string name)
Changed LoggerService.Information to LoggerService.Info
Changed 'timestamp' in FileListener to use the current Datetime.Now
Added 'launchtimestamp' in FileListener that uses the DateTime of when the app was first loaded
Fixed bug in FileListener which prevented $$[processname]$$ from being evaluated if there was no valid $$[timestamp=xxxxx]$$ to be evaluated

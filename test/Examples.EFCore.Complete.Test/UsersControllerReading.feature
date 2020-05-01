Feature: UserControllerReading
	In order to audit users
	As a user administrator
	I want to be able to search for users multiple ways

@user_read
Scenario: Administrator default read
	Given I am an administrator
	  And the default set of users exist
	 When I read users
	 Then the results should have 10 users
	  And the results should be ordered by 'LastName,FirstName'

@user_read
Scenario: Administrator read 0 users
	Given I am an administrator
	  And the default set of users exist
	 When I read 0 users
	 Then the results should have 0 users
	  And the results should be ordered by 'LastName,FirstName'

@user_read
Scenario: Administrator read 1 user
	Given I am an administrator
	  And the default set of users exist
	 When I read 1 user
	 Then the results should have 1 user
	  And the results should be ordered by 'LastName,FirstName'

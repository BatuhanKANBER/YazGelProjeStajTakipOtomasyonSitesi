$(function() {
    $('.team-table').footable({
      filtering: {
        enabled: true
      },
      "paging": {
              "enabled": true,
			  "current": 1
	  },
      strings: {
        enabled: false
      },
      "filtering": {
              "enabled": true
          },
      components: {
          filtering: FooTable.MyFiltering
      },
    }); 
  });
  
FooTable.MyFiltering = FooTable.Filtering.extend({
	construct: function(instance){
		this._super(instance);
	},
	$create: function(){
		this._super();
		var self = this;
		// create the job title form group and dropdown
		var $job_title_form_grp = $()
			.append()
			.prependTo(self.$form);

		self.$jobTitle = $('<select/>', { 'class': 'form-control ml-sm-10 ml-0' })
			.on('change', {self: self}, self._onJobTitleDropdownChanged)
			.append($('<option/>', {text: self.jobTitleDefault}))
			.appendTo($job_title_form_grp);

		$.each(self.jobTitles, function(i, jobTitle){
			self.$jobTitle.append($('<option/>').text(jobTitle));
		});
	},
	_onJobTitleDropdownChanged: function(e){
		var self = e.data.self,
			selected = $(this).val();
		if (selected !== self.jobTitleDefault){
			self.addFilter('status', selected, ['status']);
		} else {
			self.removeFilter('status');
		}
		self.filter();
	},
	draw: function(){
		this._super();
		// handle the jobTitle filter if one is supplied
		var jobTitle = this.find('status');
		if (jobTitle instanceof FooTable.Filter){
			this.$jobTitle.val(jobTitle.query.val());
		} else {
			this.$jobTitle.val(this.jobTitleDefault);
		}
	}
});
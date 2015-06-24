function GroupCollectionVM() {

    var me = new CrudVM('core', 'groupscollection', 'GroupCollectionUId');

    var allCollectionGroups;
    me.groupCollectionOutGroups = new ko.observableArray();
    me.groupCollectionInGroups = new ko.observableArray();

    me.loadallCollectionGroups = function () {
        $.ajax({
            url: _webAppUrl + 'api/core_userGroups',
            type: 'GET',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function (data) {
                allCollectionGroups = data;
                setUserGroupsIconUrl(allCollectionGroups);
            }
        });
    }

    me.onShowDetail = function () {
        me.loadUserGroups();
    }

    me.loadUserGroups = function () {
        if (me.editingItem() == null)
            return;

        if (me.newItem() != null) {
            me.editingItem().CollectionGroups = new Array();
            setgroupCollectionOutGroups();
            me.groupCollectionInGroups(me.editingItem().CollectionGroups);
            return;
        }

        $.ajax({
            url: _webAppUrl + 'api/Core_UserGroups/?fromGroupCollection=' + me.editingItem().GroupCollectionUId,
            type: 'GET',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function (data) {
                me.editingItem().CollectionGroups = data;
                setUserGroupsIconUrl(me.editingItem().CollectionGroups);
                setgroupCollectionOutGroups();
                me.groupCollectionInGroups(me.editingItem().CollectionGroups);
            }
        });
    }

    me.addGroupToUser = function (group) {
        var user = me.editingItem();
        if (group == null || user == null)
            return;

        me.groupCollectionInGroups.push(group);
        me.groupCollectionOutGroups.remove(group);
    }

    me.removeGroupFromUser = function (group) {
        var user = me.editingItem();
        if (group == null || user == null)
            return;

        me.groupCollectionOutGroups.push(group);
        me.groupCollectionInGroups.remove(group);
    }

    setUserGroupsIconUrl = function (groups) {
        for (var g in groups) {
            groups[g].iconUrl = 'url(' + _webAppUrl + 'Content/Core_Images/Groups/' + groups[g].UserGroupUId + '.png)';
        }
    }

    setgroupCollectionOutGroups = function () {
        var user = me.editingItem();
        if (allCollectionGroups == null || user == null)
            return;
        var out = allCollectionGroups.slice(0);
        for (var g in user.CollectionGroups) {
            var idx = findGroupIdx(user.CollectionGroups[g].UserGroupUId, out);
            if (idx >= 0)
                out.splice(idx, 1);
        }
        me.groupCollectionOutGroups(out);
    }

    findGroupIdx = function (groupUId, groups) {
        for (var g in groups) {
            if (groups[g].UserGroupUId == groupUId)
                return g;
        }
        return -1;
    }

    return me;
}

var pageVM = new GroupCollectionVM();
$(document).ready(function () {
    ko.applyBindings(pageVM);
    pageVM.init();
    pageVM.loadallCollectionGroups();
});



//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/role_change_request_response.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/role_grant_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'role_definition_response.g.dart';

/// RoleDefinitionResponse
///
/// Properties:
/// * [id] 
/// * [name] 
/// * [description] 
/// * [isSystem] 
/// * [isActive] 
/// * [rowVersion] 
/// * [createdAtUtc] 
/// * [memberCount] 
/// * [pendingAssignmentRequestCount] 
/// * [requiresApproval] 
/// * [permissions] 
/// * [pendingChange] 
@BuiltValue()
abstract class RoleDefinitionResponse implements Built<RoleDefinitionResponse, RoleDefinitionResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'isSystem')
  bool? get isSystem;

  @BuiltValueField(wireName: r'isActive')
  bool? get isActive;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'memberCount')
  int? get memberCount;

  @BuiltValueField(wireName: r'pendingAssignmentRequestCount')
  int? get pendingAssignmentRequestCount;

  @BuiltValueField(wireName: r'requiresApproval')
  bool? get requiresApproval;

  @BuiltValueField(wireName: r'permissions')
  BuiltList<RoleGrantResponse>? get permissions;

  @BuiltValueField(wireName: r'pendingChange')
  RoleChangeRequestResponse? get pendingChange;

  RoleDefinitionResponse._();

  factory RoleDefinitionResponse([void updates(RoleDefinitionResponseBuilder b)]) = _$RoleDefinitionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RoleDefinitionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RoleDefinitionResponse> get serializer => _$RoleDefinitionResponseSerializer();
}

class _$RoleDefinitionResponseSerializer implements PrimitiveSerializer<RoleDefinitionResponse> {
  @override
  final Iterable<Type> types = const [RoleDefinitionResponse, _$RoleDefinitionResponse];

  @override
  final String wireName = r'RoleDefinitionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RoleDefinitionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.isSystem != null) {
      yield r'isSystem';
      yield serializers.serialize(
        object.isSystem,
        specifiedType: const FullType(bool),
      );
    }
    if (object.isActive != null) {
      yield r'isActive';
      yield serializers.serialize(
        object.isActive,
        specifiedType: const FullType(bool),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.memberCount != null) {
      yield r'memberCount';
      yield serializers.serialize(
        object.memberCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.pendingAssignmentRequestCount != null) {
      yield r'pendingAssignmentRequestCount';
      yield serializers.serialize(
        object.pendingAssignmentRequestCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.requiresApproval != null) {
      yield r'requiresApproval';
      yield serializers.serialize(
        object.requiresApproval,
        specifiedType: const FullType(bool),
      );
    }
    if (object.permissions != null) {
      yield r'permissions';
      yield serializers.serialize(
        object.permissions,
        specifiedType: const FullType.nullable(BuiltList, [FullType(RoleGrantResponse)]),
      );
    }
    if (object.pendingChange != null) {
      yield r'pendingChange';
      yield serializers.serialize(
        object.pendingChange,
        specifiedType: const FullType(RoleChangeRequestResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RoleDefinitionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RoleDefinitionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'isSystem':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isSystem = valueDes;
          break;
        case r'isActive':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isActive = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'memberCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.memberCount = valueDes;
          break;
        case r'pendingAssignmentRequestCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.pendingAssignmentRequestCount = valueDes;
          break;
        case r'requiresApproval':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.requiresApproval = valueDes;
          break;
        case r'permissions':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(RoleGrantResponse)]),
          ) as BuiltList<RoleGrantResponse>?;
          if (valueDes == null) continue;
          result.permissions.replace(valueDes);
          break;
        case r'pendingChange':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(RoleChangeRequestResponse),
          ) as RoleChangeRequestResponse?;
          if (valueDes == null) continue;
          result.pendingChange.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RoleDefinitionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RoleDefinitionResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}



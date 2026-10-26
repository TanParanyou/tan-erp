//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'role_definition_summary_response.g.dart';

/// RoleDefinitionSummaryResponse
///
/// Properties:
/// * [id] 
/// * [name] 
/// * [description] 
/// * [isSystem] 
/// * [isActive] 
/// * [rowVersion] 
/// * [memberCount] 
/// * [permissionCount] 
/// * [requiresApproval] 
/// * [hasPendingChange] 
@BuiltValue()
abstract class RoleDefinitionSummaryResponse implements Built<RoleDefinitionSummaryResponse, RoleDefinitionSummaryResponseBuilder> {
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

  @BuiltValueField(wireName: r'memberCount')
  int? get memberCount;

  @BuiltValueField(wireName: r'permissionCount')
  int? get permissionCount;

  @BuiltValueField(wireName: r'requiresApproval')
  bool? get requiresApproval;

  @BuiltValueField(wireName: r'hasPendingChange')
  bool? get hasPendingChange;

  RoleDefinitionSummaryResponse._();

  factory RoleDefinitionSummaryResponse([void updates(RoleDefinitionSummaryResponseBuilder b)]) = _$RoleDefinitionSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RoleDefinitionSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RoleDefinitionSummaryResponse> get serializer => _$RoleDefinitionSummaryResponseSerializer();
}

class _$RoleDefinitionSummaryResponseSerializer implements PrimitiveSerializer<RoleDefinitionSummaryResponse> {
  @override
  final Iterable<Type> types = const [RoleDefinitionSummaryResponse, _$RoleDefinitionSummaryResponse];

  @override
  final String wireName = r'RoleDefinitionSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RoleDefinitionSummaryResponse object, {
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
    if (object.memberCount != null) {
      yield r'memberCount';
      yield serializers.serialize(
        object.memberCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.permissionCount != null) {
      yield r'permissionCount';
      yield serializers.serialize(
        object.permissionCount,
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
    if (object.hasPendingChange != null) {
      yield r'hasPendingChange';
      yield serializers.serialize(
        object.hasPendingChange,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RoleDefinitionSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RoleDefinitionSummaryResponseBuilder result,
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
        case r'memberCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.memberCount = valueDes;
          break;
        case r'permissionCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.permissionCount = valueDes;
          break;
        case r'requiresApproval':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.requiresApproval = valueDes;
          break;
        case r'hasPendingChange':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.hasPendingChange = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RoleDefinitionSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RoleDefinitionSummaryResponseBuilder();
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



//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'rename_admin_user_request.g.dart';

/// RenameAdminUserRequest
///
/// Properties:
/// * [displayName] 
@BuiltValue()
abstract class RenameAdminUserRequest implements Built<RenameAdminUserRequest, RenameAdminUserRequestBuilder> {
  @BuiltValueField(wireName: r'displayName')
  String? get displayName;

  RenameAdminUserRequest._();

  factory RenameAdminUserRequest([void updates(RenameAdminUserRequestBuilder b)]) = _$RenameAdminUserRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(RenameAdminUserRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<RenameAdminUserRequest> get serializer => _$RenameAdminUserRequestSerializer();
}

class _$RenameAdminUserRequestSerializer implements PrimitiveSerializer<RenameAdminUserRequest> {
  @override
  final Iterable<Type> types = const [RenameAdminUserRequest, _$RenameAdminUserRequest];

  @override
  final String wireName = r'RenameAdminUserRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    RenameAdminUserRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.displayName != null) {
      yield r'displayName';
      yield serializers.serialize(
        object.displayName,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    RenameAdminUserRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required RenameAdminUserRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'displayName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayName = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  RenameAdminUserRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = RenameAdminUserRequestBuilder();
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



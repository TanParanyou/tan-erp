//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'active_session_item_dto.g.dart';

/// ActiveSessionItemDto
///
/// Properties:
/// * [id] 
/// * [deviceLabel] 
/// * [clientPlatform] 
/// * [ipAddress] 
/// * [userAgent] 
/// * [createdAtUtc] 
/// * [lastUsedAtUtc] 
/// * [expiresAtUtc] 
/// * [isCurrent] 
@BuiltValue()
abstract class ActiveSessionItemDto implements Built<ActiveSessionItemDto, ActiveSessionItemDtoBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'deviceLabel')
  String? get deviceLabel;

  @BuiltValueField(wireName: r'clientPlatform')
  String? get clientPlatform;

  @BuiltValueField(wireName: r'ipAddress')
  String? get ipAddress;

  @BuiltValueField(wireName: r'userAgent')
  String? get userAgent;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'lastUsedAtUtc')
  DateTime? get lastUsedAtUtc;

  @BuiltValueField(wireName: r'expiresAtUtc')
  DateTime? get expiresAtUtc;

  @BuiltValueField(wireName: r'isCurrent')
  bool? get isCurrent;

  ActiveSessionItemDto._();

  factory ActiveSessionItemDto([void updates(ActiveSessionItemDtoBuilder b)]) = _$ActiveSessionItemDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ActiveSessionItemDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ActiveSessionItemDto> get serializer => _$ActiveSessionItemDtoSerializer();
}

class _$ActiveSessionItemDtoSerializer implements PrimitiveSerializer<ActiveSessionItemDto> {
  @override
  final Iterable<Type> types = const [ActiveSessionItemDto, _$ActiveSessionItemDto];

  @override
  final String wireName = r'ActiveSessionItemDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ActiveSessionItemDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.deviceLabel != null) {
      yield r'deviceLabel';
      yield serializers.serialize(
        object.deviceLabel,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.clientPlatform != null) {
      yield r'clientPlatform';
      yield serializers.serialize(
        object.clientPlatform,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.ipAddress != null) {
      yield r'ipAddress';
      yield serializers.serialize(
        object.ipAddress,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.userAgent != null) {
      yield r'userAgent';
      yield serializers.serialize(
        object.userAgent,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.lastUsedAtUtc != null) {
      yield r'lastUsedAtUtc';
      yield serializers.serialize(
        object.lastUsedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.expiresAtUtc != null) {
      yield r'expiresAtUtc';
      yield serializers.serialize(
        object.expiresAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.isCurrent != null) {
      yield r'isCurrent';
      yield serializers.serialize(
        object.isCurrent,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ActiveSessionItemDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ActiveSessionItemDtoBuilder result,
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
        case r'deviceLabel':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.deviceLabel = valueDes;
          break;
        case r'clientPlatform':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.clientPlatform = valueDes;
          break;
        case r'ipAddress':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.ipAddress = valueDes;
          break;
        case r'userAgent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.userAgent = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'lastUsedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.lastUsedAtUtc = valueDes;
          break;
        case r'expiresAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.expiresAtUtc = valueDes;
          break;
        case r'isCurrent':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isCurrent = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ActiveSessionItemDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ActiveSessionItemDtoBuilder();
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


